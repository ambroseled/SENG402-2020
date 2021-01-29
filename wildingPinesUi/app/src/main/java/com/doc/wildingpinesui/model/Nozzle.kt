package com.doc.wildingpinesui.model

/**
 * A nozzle in the spray boom.
 */
data class Nozzle(
    /**
     * An identifier for the nozzle.
     */
    val id: Int,

    /**
     * Whether the boom should be spraying or not, only updated when testing nozzles. Not during
     * a spraying operation.
     */
    var shouldBeSpraying: Boolean,

    /**
     * A name that can be displayed to the user.
     */
    val displayName: String
)