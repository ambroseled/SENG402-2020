package com.doc.wildingpinesui.network.datasources

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import com.doc.wildingpinesui.model.SprayPlan
import com.doc.wildingpinesui.model.SprayReport
import com.doc.wildingpinesui.model.UsbDrive

/**
 * Used to sync files to and from the NUC.
 */
interface ISyncFilesDataSource {
    /**
     * Whether a request is currently in progress to update the available USB drives.
     */
    val isLoadingUsbDrives: LiveData<Boolean>

    /**
     * The cached available USB drives.
     */
    val downloadedAvailableUsbDrives: LiveData<List<UsbDrive>>

    /**
     * The spray plan files in the USB.
     */
    val downloadedSprayPlansInUsbDrive: LiveData<List<SprayPlan>>

    /**
     * The spray plans available in the NUC.
     */
    val sprayPlansInNuc: LiveData<List<SprayPlan>>

    /**
     * The spray reports available in the NUC.
     */
    val sprayReportsInNuc: LiveData<List<SprayReport>>

    /**
     * Update the available spray reports in the NUC.
     */
    suspend fun fetchAvailableSprayReportsInNuc()

    /**
     * Update the available USB drives.
     */
    suspend fun fetchCurrentAvailableUsbDrives()

    /**
     * Update the available spray plans in the USB.
     */
    suspend fun fetchSprayPlansInUsb(usbName: String)

    /**
     * Update the available spray plans in the NUC.
     */
    suspend fun fetchAvailableSprayPlansInNuc()

    /**
     * Import the specified files from the USB to the NUC.
     */
    suspend fun importSprayPlansFromUsbToNuc(usbName: String, filepaths: List<String>)

    /**
     * Import all the spray plans from the USB to the NUC.
     */
    suspend fun importAllSprayPlansFromUsbToNuc(usbName: String)

    /**
     * Export the spray reports from the NUC to a USB.
     */
    suspend fun exportSprayReportsFromNucToUsb(usbName: String, reportNames: List<String>)

    /**
     * Export all spray reports from the NUC to a USB.
     */
    suspend fun exportAllSprayReportsFromNucToUsb(usbName: String)
}