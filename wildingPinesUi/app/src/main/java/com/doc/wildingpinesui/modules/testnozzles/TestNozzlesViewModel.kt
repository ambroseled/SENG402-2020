package com.doc.wildingpinesui.modules.testnozzles

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.doc.wildingpinesui.model.Nozzle
import com.doc.wildingpinesui.network.datasources.INozzlesDataSource
import kotlinx.coroutines.launch

/**
 * Stores and updates data used by the UI in the 'Test nozzles' section of the app.
 */
class TestNozzlesViewModel(private val nozzlesDataSource: INozzlesDataSource) : ViewModel() {
    private val _nozzlesInTheNuc: MutableLiveData<List<Nozzle>> = MutableLiveData(listOf())

    val nozzlesInTheNuc: LiveData<List<Nozzle>>
        get() = _nozzlesInTheNuc

    init {
        viewModelScope.launch {
            nozzlesDataSource.fetchAvailableNozzles()

            nozzlesDataSource.nozzlesInTheNuc.observeForever {
                _nozzlesInTheNuc.postValue(it)
            }
        }
    }

    /**
     * @return whether the operation succeeded.
     */
    suspend fun fetchNozzlesInTheNuc(): Boolean {
        return nozzlesDataSource.fetchAvailableNozzles()
    }

    /**
     * @return whether the operation succeeded.
     */
    suspend fun updateNozzleStatuses(nozzles: List<Nozzle>): Boolean {
        return nozzlesDataSource.updateNozzleStatuses(nozzles)
    }
}