package com.doc.wildingpinesui.network.datasources

import android.util.Log
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import com.doc.wildingpinesui.network.apiservices.ISprayPlansApiService
import com.doc.wildingpinesui.model.SprayPlan
import com.doc.wildingpinesui.model.SprayReport
import com.doc.wildingpinesui.model.UsbDrive
import com.doc.wildingpinesui.network.apiservices.ISprayReportsApiService
import com.haroldadmin.cnradapter.NetworkResponse

/**
 * Used to sync files to and from the NUC.
 */
class SyncFilesDataSource(private val sprayPlansApiService: ISprayPlansApiService, private val sprayReportsApiService: ISprayReportsApiService) :
    ISyncFilesDataSource {
    private val _isLoadingUsbDrives = MutableLiveData<Boolean>(false)
    private val _downloadedAvailableUsbDrives = MutableLiveData<List<UsbDrive>>()
    private val _downloadedSprayPlansInUsbDrive = MutableLiveData<List<SprayPlan>>()
    private val _sprayPlansInNuc = MutableLiveData<List<SprayPlan>>()
    private val _sprayReportsInNuc = MutableLiveData<List<SprayReport>>()

    override val isLoadingUsbDrives: LiveData<Boolean>
        get() = _isLoadingUsbDrives

    override val downloadedAvailableUsbDrives: LiveData<List<UsbDrive>>
        get() = _downloadedAvailableUsbDrives

    override val downloadedSprayPlansInUsbDrive: LiveData<List<SprayPlan>>
        get() = _downloadedSprayPlansInUsbDrive

    override val sprayPlansInNuc: LiveData<List<SprayPlan>>
        get() = _sprayPlansInNuc

    override val sprayReportsInNuc: LiveData<List<SprayReport>>
        get() = _sprayReportsInNuc

    override suspend fun fetchAvailableSprayReportsInNuc() {
        val networkResponse = sprayReportsApiService.getReportsAvailableInNuc()
        when (networkResponse) {
            is NetworkResponse.Success -> _sprayReportsInNuc.postValue(networkResponse.body)
            is NetworkResponse.UnknownError -> Log.e(this::class.java.simpleName, "Could not fetch available spray reports in NUC", networkResponse.error)
            else -> Log.e(this::class.java.simpleName, "Could not fetch available spray reports in NUC: $networkResponse")
        }
    }

    override suspend fun fetchCurrentAvailableUsbDrives() {
        _isLoadingUsbDrives.postValue(true)
        val networkResponse = sprayPlansApiService.getAvailableUsbDrives()
        when (networkResponse) {
            is NetworkResponse.Success -> _downloadedAvailableUsbDrives.postValue(networkResponse.body)
            else -> Log.e(this::class.java.simpleName, "Could not fetch available USB drives: $networkResponse")
        }
        _isLoadingUsbDrives.postValue(false)
    }

    override suspend fun fetchSprayPlansInUsb(usbName: String) {
        val networkResponse = sprayPlansApiService.getSprayPlansInUsb(usbName)
        when (networkResponse) {
            is NetworkResponse.Success -> _downloadedSprayPlansInUsbDrive.postValue(networkResponse.body)
            else -> Log.e(this::class.java.simpleName, "Could not fetch spray plans in USB: $networkResponse")
        }
    }

    override suspend fun fetchAvailableSprayPlansInNuc() {
        val networkResponse = sprayPlansApiService.getAvailablePlansInNuc()
        when (networkResponse) {
            is NetworkResponse.Success -> _sprayPlansInNuc.postValue(networkResponse.body)
            else -> Log.e(this::class.java.simpleName, "Something went wrong fetching available spray plans in NUC: $networkResponse")
        }
    }

    override suspend fun importSprayPlansFromUsbToNuc(usbName: String, filepaths: List<String>) {
        val networkResponse = sprayPlansApiService.importSprayPlansFromUsbToNuc(usbName, filepaths)
        when (networkResponse) {
            is NetworkResponse.Success -> Log.d(this::class.java.simpleName, networkResponse.body.message)
            else -> Log.e(this::class.java.simpleName, "Could not import plans $filepaths from USB $usbName to NUC: $networkResponse")
        }
    }

    override suspend fun importAllSprayPlansFromUsbToNuc(usbName: String) {
        val networkResponse = sprayPlansApiService.importAllSprayPlansFromUsbToNuc(usbName)
        when (networkResponse) {
            is NetworkResponse.Success -> Log.d(this::class.java.simpleName, networkResponse.body.message)
            else -> Log.e(this::class.java.simpleName, "Could not import all plans from usb $usbName: $networkResponse")
        }
    }

    override suspend fun exportSprayReportsFromNucToUsb(
        usbName: String,
        reportNames: List<String>
    ) {
        val networkResponse = sprayReportsApiService.exportSprayReportsFromNucToUsb(usbName, reportNames)
        when (networkResponse) {
            is NetworkResponse.Success -> Log.d(this::class.java.simpleName, networkResponse.body.message)
            else -> Log.e(this::class.java.simpleName, "Could not export spray reports to usb $usbName: $networkResponse")
        }
    }

    override suspend fun exportAllSprayReportsFromNucToUsb(usbName: String) {
        val networkResponse = sprayReportsApiService.exportAllSprayReportsFromNucToUsb(usbName)
        when (networkResponse) {
            is NetworkResponse.Success -> Log.d(this::class.java.simpleName, networkResponse.body.message)
            else -> Log.e(this::class.java.simpleName, "Could not export spray reports to usb $usbName: $networkResponse")
        }
    }
}