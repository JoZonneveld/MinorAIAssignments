//import java.io.File
//import kotlin.random.Random
//
//fun main(args: Array<String>) {
//    val wineData = File("Clustering/src/resource/winedata.csv")
//            .readLines()
//            .withIndex()
//            .filter { (index, _) -> index > 0 }
//            .map { (_, it) -> Winedata(it.split(",").toList()) }
//    val clientData = File("Clustering/src/resource/transactions.csv")
//            .readLines()
//            .withIndex()
//            .filter { (index, _) -> index > 0 }
//            .map { (_, it) -> Wineclient(it.split(",").toList()) }
//
//    val clientDataGroup = clientData.sortedBy { it.customerLastName }.groupBy { it.customerLastName }
//
//    val clientWineChoices = clientDataGroup.map { (key, value) ->
//        val offers = value.map { it.offerId }
//        key to wineData.map { wine ->
//            WineChoice(
//                    wine.offerId,
//                    if (wine.offerId in offers) 1.0 else 0.0
//            )
//        }
//    }
//
//    val calculateDistance: (List<WineChoice>, List<WineChoice>) -> Double = { centroid, data ->
//        val dataValues = data.map { it.choice }
//        val centroidValues = centroid.map { it.choice }
//        val zippedValues = dataValues.zip(centroidValues).map { Math.pow(it.first - it.second, 2.0) }.sum()
//        Math.sqrt(zippedValues)
//    }
//
//    val res = kMeans(
//            5,
//            10000,
//            clientWineChoices.map { it.second },
//            { k, dataset ->
//                val values = dataset.first().map { it.offerId }
////            val list1 = listOf(
////                0.028,
////                0.234,
////                0.017,
////                0.016,
////                0.023,
////                0.028,
////                0.034,
////                0.007,
////                0.014,
////                0.010,
////                0.032,
////                0.016,
////                0.026,
////                0.045,
////                0.013,
////                0.030,
////                0.608,
////                0.027,
////                0.025,
////                0.024,
////                0.014,
////                0.009,
////                0.029,
////                0.941,
////                0.025,
////                0.690,
////                0.010,
////                0.026,
////                0.012,
////                0.020,
////                0.023,
////                0.093
////            )
////            val list2 = listOf(
////                0.012,
////                0.022,
////                0.023,
////                0.026,
////                0.022,
////                0.010,
////                0.541,
////                0.430,
////                0.014,
////                0.041,
////                0.025,
////                0.041,
////                0.194,
////                0.034,
////                0.024,
////                0.009,
////                0.032,
////                0.419,
////                0.015,
////                0.015,
////                0.049,
////                0.024,
////                0.023,
////                0.043,
////                0.034,
////                0.030,
////                0.021,
////                0.017,
////                0.619,
////                0.729,
////                0.027,
////                0.013
////            )
////            val list3 = listOf(
////                0.043,
////                0.108,
////                0.054,
////                0.130,
////                0.054,
////                0.095,
////                0.123,
////                0.155,
////                0.141,
////                0.083,
////                0.128,
////                0.073,
////                0.016,
////                0.061,
////                0.052,
////                0.063,
////                0.005,
////                0.074,
////                0.078,
////                0.069,
////                0.050,
////                0.023,
////                0.036,
////                0.017,
////                0.083,
////                0.090,
////                0.087,
////                0.090,
////                0.043,
////                0.079,
////                0.211,
////                0.053
////            )
////            val list4 = listOf(
////                0.275,
////                0.115,
////                0.160,
////                0.174,
////                0.057,
////                0.297,
////                0.086,
////                0.122,
////                0.114,
////                0.084,
////                0.280,
////                0.088,
////                0.011,
////                0.163,
////                0.171,
////                0.027,
////                0.061,
////                0.054,
////                0.071,
////                0.062,
////                0.069,
////                0.951,
////                0.062,
////                0.035,
////                0.114,
////                0.130,
////                0.141,
////                0.030,
////                0.038,
////                0.136,
////                0.259,
////                0.125
////            )
//
////            listOf(
////                values.zip(list1).map { WineChoice(it.first, it.second) },
////                values.zip(list2).map { WineChoice(it.first, it.second) },
////                values.zip(list3).map { WineChoice(it.first, it.second) },
////                values.zip(list4).map { WineChoice(it.first, it.second) }
////            )
//
//                IntRange(1, k).map {
//                    values.map { offerId ->
//                        WineChoice(
//                                offerId,
//                                Random.nextDouble()
//                        )
//                    }
//                }
//            },
//            calculateDistance,
//            { centroid, data ->
//                val groupedOffers = data.flatten().groupBy { it.offerId }
//                centroid.map {
//                    WineChoice(
//                            it.offerId,
//                            groupedOffers[it.offerId]!!.map { client -> client.choice }.sum().div(data.size.toDouble())
//                    )
//                }
//            }
//    )
//
//    val clientClosestToCluster = clientWineChoices.map { client ->
//        val c = res.withIndex().map { (index, centroid) -> index to calculateDistance(centroid, client.second) }
//        val b = c.sortedBy { it.second }
//        val a = b.first().first
//        client to a
//    }
//
//    res.withIndex().forEach { (index, it) ->
//        println("# Cluster ${index + 1}")
//        println("|Offer #|Choice|Wine|")
//        println("|:-|:-|:-|")
//
//        val sorted = it.map { wine ->
//            wine.offerId to clientClosestToCluster
//                    .filter { (_, clientIndex) -> clientIndex == index }
//                    .sumByDouble { (pair, _) ->
//                        val (_, values) = pair
//                        values.filter { it.offerId == wine.offerId }.sumByDouble { it.choice }
//                    }
//        }.sortedByDescending { it.second }
//        sorted.forEach { (offerId, choice) ->
//            val wine = wineData.find { it.offerId == offerId }!!.varietal
//            println("|$offerId|$choice|$wine|")
//        }
//    }
//}
//
//data class Winedata(
//        val offerId: Int,
//        val campaign: String,
//        val varietal: String,
//        val minimumQuantityKg: Int,
//        val discountPercentage: Int,
//        val origin: String,
//        val pastPeak: Boolean
//) {
//    constructor(list: List<String>) : this(
//            list[0].toInt(),
//            list[1],
//            list[2],
//            list[3].toInt(),
//            list[4].toInt(),
//            list[5],
//            list[6].let { it == "TRUE" }
//    )
//}
//
//data class Wineclient(
//        val customerLastName: String,
//        val offerId: Int
//) {
//    constructor(list: List<String>) : this(
//            list[0],
//            list[1].toInt()
//    )
//}
//
//data class WineChoice(
//        val offerId: Int,
//        val choice: Double
//)
//
//fun <T> kMeans(
//        k: Int,
//        iterations: Int,
//        dataset: List<T>,
//        initialize: (Int, List<T>) -> List<T>,
//        calculateDistance: (T, T) -> Double,
//        calculateMeanCentroid: (T, List<T>) -> T
//): List<T> {
//    val centroidsMap: HashMap<T, MutableList<T>> = HashMap()
//
//    var centroids = initialize(k, dataset)
//    val oldCentroids: MutableList<List<T>> = mutableListOf()
//
//    var currentIteration = 0
//    while (currentIteration < iterations && centroids !in oldCentroids) {
//        currentIteration++
//        oldCentroids.add(centroids)
//
//        centroidsMap.clear()
//        // for each data point
//        dataset.forEach { data ->
//            // - find nearest centroid
//            val nearest = centroids.sortedBy { centroid -> calculateDistance(centroid, data) }.first()
//            // - assign the point to that cluster
//            centroidsMap[nearest]?.run { this.add(data) } ?: run { centroidsMap[nearest] = mutableListOf(data) }
//        }
//
//        // for each cluster
//        centroids = centroids.map { centroid ->
//            // - new centroid = mean of all points assigned to that cluster
//            centroidsMap[centroid]?.let { list ->
//                calculateMeanCentroid(centroid, list)
//            } ?: centroid
//        }
//    }
//
//    return centroids
//}