import java.io.File
import kotlin.random.Random

fun main(args: Array<String>) {
    val wines = File("Clustering/src/resource/winedata.csv")
            .readLines()
            .withIndex()
            .filter { (index, _ )-> index > 0 }
            .map { (_, it) -> Wine(it.split(",").toList()) }

    val transactions =  File("Clustering/src/resource/transactions.csv")
            .readLines()
            .withIndex()
            .filter { (index, _) -> index > 0 }
            .map { (_, it) -> Transaction(it.split(",").toList()) }

    val orderdTransactions = transactions.sortedBy { it.name }.groupBy { it.name }

    val clientWineChoices = orderdTransactions.map { (key, value) ->
        val offers = value.map { it.transactionId }
        key to wines.map { wine ->
            WineChoice(
                    wine.offerId,
                    if (wine.offerId in offers) 1.0 else 0.0
            )
        }
    }

    val dist: (List<WineChoice>, List<WineChoice>) -> Double = { centroid, data ->
        val dataValues = data.map { it.choice }
        val centroidValues = centroid.map { it.choice }
        val zippedValues = dataValues.zip(centroidValues).map { Math.pow(it.first - it.second, 2.0) }.sum()
        Math.sqrt(zippedValues)
    }



    val res = kMeans(
            k = 5,
            iterations = 10000,
            dataset = clientWineChoices.map { it.second },
            initialize = {k, dataset ->
                val values = dataset.first().map { it.offerId }
                IntRange(1, k).map {
                    values.map { offerId ->
                        WineChoice(
                                offerId,
                                Random.nextDouble()
                        )
                    }
                }
            },
            calculateDistance = dist,
            calculateMeanCentroid = {centroid, dataset ->
                val groupedOffers = dataset.flatten().groupBy { it.offerId }
                centroid.map {
                    WineChoice(
                            it.offerId,
                            requireNotNull(groupedOffers[it.offerId]).map { client -> client.choice }.sum().div(dataset.size.toDouble())
                    )
                }
            }
    )

    val clientClosestToCluster = clientWineChoices.map { client ->
        val c = res.withIndex().map { (index, centroid) -> index to dist(centroid, client.second) }
        val b = c.sortedBy { it.second }
        val a = b.first().first
        client to a
    }

    res.withIndex().forEach { (index, it) ->
        println("# Cluster ${index + 1}")
        println("|Offer #|Choice|Wine|")
        println("|:-|:-|:-|")

        val sorted = it.map { wine ->
            wine.offerId to clientClosestToCluster
                    .filter { (_, clientIndex) -> clientIndex == index }
                    .sumByDouble { (pair, _) ->
                        val (_, values) = pair
                        values.filter { it.offerId == wine.offerId }.sumByDouble { it.choice }
                    }
        }.sortedByDescending { it.second }
        sorted.forEach { (offerId, choice) ->
            val wine = wines.find { it.offerId == offerId }!!.varietal
            println("|$offerId|$choice|$wine|")
        }
    }
    println()
}


fun <T> kMeans(
        k: Int,
        iterations: Int,
        dataset: List<T>,
        initialize: (Int, List<T>) -> List<T>,
        calculateDistance: (T, T) -> Double,
        calculateMeanCentroid: (T, List<T>) -> T
): List<T> {
    val centroidsMap: HashMap<T, MutableList<T>> = HashMap()

    var centroids = initialize(k, dataset)
    val oldCentroids: MutableList<List<T>> = mutableListOf()

    var currentIteration = 0
    while (currentIteration < iterations && centroids !in oldCentroids) {
        currentIteration++
        oldCentroids.add(centroids)

        centroidsMap.clear()
        // for each data point
        dataset.forEach { data ->
            // - find nearest centroid
            val nearest = centroids.sortedBy { centroid -> calculateDistance(centroid, data) }.first()
            // - assign the point to that cluster
            centroidsMap[nearest]?.run { this.add(data) } ?: run { centroidsMap[nearest] = mutableListOf(data) }
        }

        // for each cluster
        centroids = centroids.map { centroid ->
            // - new centroid = mean of all points assigned to that cluster
            centroidsMap[centroid]?.let { list ->
                calculateMeanCentroid(centroid, list)
            } ?: centroid
        }
    }

    return centroids
}

class WineChoice(val offerId : Int, val choice: Double)

class Transaction(val name : String, val transactionId : Int){

    constructor(list : List<String>) : this(list[0], list[1].toInt())
}

data class Wine(
        val offerId: Int,
        val campaign: String,
        val varietal: String,
        val minimumQty: Int,
        val discount : Int,
        val origin : String,
        val pastPeak : Boolean
){
    constructor(list : List<String>) : this(
            list[0].toInt(), list[1], list[2], list[3].toInt(), list[4].toInt(), list[5], list[6].toBoolean())
}