import kotlin.random.Random

fun main(args: Array<String>) {
    val population = 100
    val individuals = IntRange(1, population).map { createIndividual() }
    val fitnesses = individuals.map { computeFitness(it) }
    val parents = selectTwoParents(individuals, fitnesses)
    val crossover = crossover(parents);
    println(parents)
}

fun createIndividual(): String {
    var output = ""
    val random = Random
    for (i in 0..4) {
        output += random.nextInt(0, 2)
    }

    return output
}

fun computeFitness(indu: String): Double {
    var fitness = 0.0
    val x = indu.toInt(2).toDouble()
    fitness = -(Math.pow(x, 2.0)) + (7 * x)

    return fitness
}

fun selectTwoParents(individuals: List<String>, fitness: List<Double>): List<String> {
    val minFitness = fitness.min()!!
    var cursor = minFitness

    val individualsWithFitness = individuals.zip(fitness)
    val individualsWithRanges = individualsWithFitness.map { (ind, fitness) ->
        val range = fitness + Math.abs(Math.min(minFitness, 0.0))
        val cursorEnd = cursor + range
        val res = (cursor to cursorEnd) to ind
        cursor += range
        res
    }

    return IntRange(1, 2).map {
        val point = Random.nextDouble(minFitness, cursor)
        individualsWithRanges.find { (range, _) ->
            range.first <= point && point < range.second
        }?.second ?: individualsWithRanges.first().second
    }
}

fun crossover(parents: List<String>): List<String> {
    val l = parents[0].length
    val pointer = Random.nextInt(1, l - 1)
    return listOf(
        parents[0].substring(0, pointer) + parents[1].substring(pointer, l),
        parents[1].substring(0, pointer) + parents[0].substring(pointer, l)
    )
}