package com.doc.wildingpinesui.modules.syncfiles

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.doc.wildingpinesui.model.SprayPlan
import com.doc.wildingpinesui.model.SprayReport
import com.doc.wildingpinesui.model.UsbDrive
import com.doc.wildingpinesui.network.datasources.ISyncFilesDataSource
import kotlinx.coroutines.launch

/**
 * Stores and updates data used by the UI in the 'Sync files' section of the app.
 */
class SyncFilesViewModel(private val syncFilesDataSource: ISyncFilesDataSource) : ViewModel() {
    private val _isLoadingUsbDrives = MutableLiveData<Boolean>(false)
    private val _downloadedAvailableUsbDrives = MutableLiveData<List<UsbDrive>>()
    private val _downloadedSprayPlansInUsbDrive = MutableLiveData<List<SprayPlan>>()
    private val _sprayPlansInNuc = MutableLiveData<List<SprayPlan>>()
    private val _sprayReportsInNuc = MutableLiveData<List<SprayReport>>()

    init {
        viewModelScope.launch {
            fetchCurrentAvailableUsbDrives()
            fetchAvailableSprayPlansInNuc()
            fetchAvailableSprayReportsInNuc()

            syncFilesDataSource.isLoadingUsbDrives.observeForever {
                _isLoadingUsbDrives.postValue(it)
            }
            syncFilesDataSource.downloadedAvailableUsbDrives.observeForever {
                _downloadedAvailableUsbDrives.postValue(it)
            }
            syncFilesDataSource.downloadedSprayPlansInUsbDrive.observeForever {
                _downloadedSprayPlansInUsbDrive.postValue(it)
            }
            syncFilesDataSource.sprayPlansInNuc.observeForever {
                _sprayPlansInNuc.postValue(it)
            }
            syncFilesDataSource.sprayReportsInNuc.observeForever {
                _sprayReportsInNuc.postValue(it)
            }
        }
    }

    val isLoadingUsbDrives: LiveData<Boolean>
        get() = _isLoadingUsbDrives

    val downloadedAvailableUsbDrives: LiveData<List<UsbDrive>>
        get() = _downloadedAvailableUsbDrives

    val downloadedSprayPlansInUsbDrive: LiveData<List<SprayPlan>>
        get() = _downloadedSprayPlansInUsbDrive

    val sprayPlansInNuc: LiveData<List<SprayPlan>>
        get() = _sprayPlansInNuc

    val sprayReportsInNuc: LiveData<List<SprayReport>>
        get() = _sprayReportsInNuc

    suspend fun fetchCurrentAvailableUsbDrives() {
        syncFilesDataSource.fetchCurrentAvailableUsbDrives()
    }

    suspend fun fetchSprayPlansInUsb(usbName: String) {
        syncFilesDataSource.fetchSprayPlansInUsb(usbName)
    }

    suspend fun fetchAvailableSprayPlansInNuc() {
        syncFilesDataSource.fetchAvailableSprayPlansInNuc()
    }

    suspend fun fetchAvailableSprayReportsInNuc() {
        syncFilesDataSource.fetchAvailableSprayReportsInNuc()
    }

    suspend fun importSprayPlansFromUsbToNuc(usbName: String, filenames: List<String>) {
        syncFilesDataSource.importSprayPlansFromUsbToNuc(usbName, filenames)
    }

    suspend fun importAllSprayPlansFromUsbToNuc(usbName: String) {
        syncFilesDataSource.importAllSprayPlansFromUsbToNuc(usbName)
    }

    suspend fun exportSprayReportsFromNucToUsb(usbName: String, reportNames: List<String>) {
        syncFilesDataSource.exportSprayReportsFromNucToUsb(usbName, reportNames)
    }

    suspend fun exportAllSprayReportsFromNucToUsb(usbName: String) {
        syncFilesDataSource.exportAllSprayReportsFromNucToUsb(usbName)
    }
}