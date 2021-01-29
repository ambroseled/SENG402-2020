package com.doc.wildingpinesui.modules.syncfiles

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.doc.wildingpinesui.network.datasources.ISyncFilesDataSource

/**
 * Used to construct a [SyncFilesViewModel] for dependency injection.
 * Created using this resource:
 * https://www.youtube.com/watch?v=DwnloROxaKg&list=PLB6lc7nQ1n4jTLDyU2muTBo8xk0dg0D_w&index=6
 */
class SyncFilesViewModelFactory(private val syncFilesDataSource: ISyncFilesDataSource) : ViewModelProvider.NewInstanceFactory() {

    @Suppress("UNCHECKED_CAST")
    override fun <T : ViewModel?> create(modelClass: Class<T>): T {
        return SyncFilesViewModel(syncFilesDataSource) as T
    }
}