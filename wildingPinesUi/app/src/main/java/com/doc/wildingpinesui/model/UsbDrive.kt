package com.doc.wildingpinesui.model

import com.google.gson.annotations.SerializedName

/**
 * A USB drive connected to the NUC.
 */
data class UsbDrive(
    @SerializedName("name")
    val name: String
)