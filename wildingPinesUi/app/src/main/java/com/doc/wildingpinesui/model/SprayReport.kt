package com.doc.wildingpinesui.model

import java.util.*

/**
 * A report of the spraying done after a flight.
 */
data class SprayReport(
    val name: String,
    val sprayUsed: Long,
    val flightTime: Long,
    val treesSprayed: Long,
    val created: Date
)