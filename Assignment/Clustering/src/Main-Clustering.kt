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

    val centroids = computeRandomCentroids(5)
    val t = clientWineChoices.map { it.second }.forEach { dist(centroids, it)}

    println()

}

fun dist(centroids: List<Double>, wineChoices : List<WineChoice>) {
    val dataValues = wineChoices.map { it.choice }
    val zippedValues = dataValues.zip(centroids).map { Math.pow(it.first - it.second, 2.0) }.sum()
    val res = Math.sqrt(zippedValues)
    println()
}

fun <A> clustering(k : Int, iterations: Int, dataset: A){



    var centroids = computeRandomCentroids(k)
    val oldCentroids: MutableList<List<A>> = mutableListOf()
    var i = 0
    while(i < iterations && centroids !in oldCentroids){
        i++
    }
}

fun computeRandomCentroids(k: Int): List<Double> {
    val centroids = mutableListOf<Double>()

    for(i in 0 until k){
        centroids.add(Random.nextDouble(1.0))
    }

    return centroids
}

fun <A> reComputeCentroid(centroid : A, cluster: List<A>) : A {


    return centroid
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