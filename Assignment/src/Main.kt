import kotlin.random.Random

fun main(args: Array<String>) {
    println(computeFitness(createIndividual()))
}

fun createIndividual() : String{
    var output = ""
    val random = Random
    for (i in 0..4){
        output += random.nextInt(0,2)
    }

    return output
}

fun computeFitness(indu: String) : Double{
    var fitness = 0.0
    try {
        val x = indu.toDouble()
        fitness = -(Math.pow(x, 2.0)) +  (7*x)
    } catch (e: Exception){
        throw e
    }
    return fitness
}