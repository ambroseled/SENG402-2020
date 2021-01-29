package com.doc.wildingpinesui

import android.util.Log
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.doc.wildingpinesui.model.SystemStatus
import com.doc.wildingpinesui.network.datasources.IStateChangeDataSource
import kotlinx.coroutines.launch
import java.util.*
import kotlin.concurrent.scheduleAtFixedRate

/**
 * Stores and updates data used by the UI in the [MainActivity].
 */
class MainActivityViewModel(private val stateChangeDataSource: IStateChangeDataSource) : ViewModel() {
    private val _systemStatus = MutableLiveData(SystemStatus.NUC_UNREACHABLE)
    private val _lastStatusUpdate: MutableLiveData<Date?> = MutableLiveData(null)
    private val systemStatusUpdateTimer = Timer()

    val systemStatus: LiveData<SystemStatus>
        get() = _systemStatus

    val lastStatusUpdate: LiveData<Date?>
        get() = _lastStatusUpdate

    init {
        viewModelScope.launch {
            stateChangeDataSource.updateSystemStatus()

            // set up a task to periodically update system status
            systemStatusUpdateTimer.scheduleAtFixedRate(10000, 5000) {
                viewModelScope.launch {
                    Log.d(this::class.java.simpleName, "Requested health check from NUC")
                    updateSystemStatus()
                }
            }
        }

        stateChangeDataSource.systemStatus.observeForever {
            _systemStatus.postValue(it)
        }
        stateChangeDataSource.lastStatusUpdate.observeForever {
            _lastStatusUpdate.postValue(it)
        }
    }

    /**
     * Request the NUC to shut down the whole system.
     * @return whether the NUC received the shutdown request.
     */
    suspend fun requestNucShutdown(): Boolean {
        return stateChangeDataSource.requestNucShutdown()
    }

    /**
     * Update the status for the whole system.
     */
    private suspend fun updateSystemStatus() {
        stateChangeDataSource.updateSystemStatus()
    }
}