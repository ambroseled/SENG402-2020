package com.doc.wildingpinesui.modules.testnozzles

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.doc.wildingpinesui.network.datasources.INozzlesDataSource

/**
 * Used to construct a [TestNozzlesViewModel] for dependency injection.
 * Created using this resource:
 * https://www.youtube.com/watch?v=DwnloROxaKg&list=PLB6lc7nQ1n4jTLDyU2muTBo8xk0dg0D_w&index=6
 */
class TestNozzlesViewModelFactory(private val nozzlesDataSource: INozzlesDataSource) : ViewModelProvider.NewInstanceFactory() {
    @Suppress("UNCHECKED_CAST")
    override fun <T : ViewModel?> create(modelClass: Class<T>): T {
        return TestNozzlesViewModel(nozzlesDataSource) as T
    }
}