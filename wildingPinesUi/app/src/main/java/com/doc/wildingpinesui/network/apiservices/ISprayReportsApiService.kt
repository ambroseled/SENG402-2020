package com.doc.wildingpinesui.network.apiservices

import com.doc.wildingpinesui.model.ErrorResponse
import com.doc.wildingpinesui.model.SprayReport
import com.doc.wildingpinesui.model.SuccessResponse
import com.haroldadmin.cnradapter.NetworkResponse
import com.haroldadmin.cnradapter.NetworkResponseAdapterFactory
import com.jakewharton.retrofit2.adapter.kotlin.coroutines.CoroutineCallAdapterFactory
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Query

/**
 * Used to do API calls to do with spray reports.
 */
interface ISprayReportsApiService {

    @GET("reports")
    suspend fun getReportsAvailableInNuc(): NetworkResponse<List<SprayReport>, ErrorResponse>

    @POST("reports/export/usb")
    suspend fun exportSprayReportsFromNucToUsb(
        @Query("usbname")
        usbName: String,

        @Body reportNames: List<String>
    ): NetworkResponse<SuccessResponse, ErrorResponse>

    @POST("reports/export/all/usb")
    suspend fun exportAllSprayReportsFromNucToUsb(
        @Query("usbname")
        usbName: String
    ): NetworkResponse<SuccessResponse, ErrorResponse>

    companion object {
        operator fun invoke(): ISprayReportsApiService {
            return Retrofit
                .Builder()
                .baseUrl(ApiHostSettings.apiBaseUrl)
                .addCallAdapterFactory(CoroutineCallAdapterFactory())
                .addCallAdapterFactory(NetworkResponseAdapterFactory())
                .addConverterFactory(GsonConverterFactory.create())
                .build()
                .create(ISprayReportsApiService::class.java)
        }
    }
}