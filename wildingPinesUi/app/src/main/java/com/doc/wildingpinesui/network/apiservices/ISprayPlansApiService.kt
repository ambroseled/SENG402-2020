package com.doc.wildingpinesui.network.apiservices

import com.doc.wildingpinesui.model.ErrorResponse
import com.doc.wildingpinesui.model.SprayPlan
import com.doc.wildingpinesui.model.SuccessResponse
import com.doc.wildingpinesui.model.UsbDrive
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
 * Used to make API calls to do with spray plans.
 */
interface ISprayPlansApiService {

    @GET("usb/available")
    suspend fun getAvailableUsbDrives(): NetworkResponse<List<UsbDrive>, ErrorResponse>

    @GET("plans/usb")
    suspend fun getSprayPlansInUsb(
        @Query("usbname")
        usbName: String
    ): NetworkResponse<List<SprayPlan>, ErrorResponse>

    @GET("plans")
    suspend fun getAvailablePlansInNuc(): NetworkResponse<List<SprayPlan>, ErrorResponse>

    @POST("plans/import/usb")
    suspend fun importSprayPlansFromUsbToNuc(
        @Query("usbname")
        usbName: String,

        @Body
        filenames: List<String>
    ): NetworkResponse<SuccessResponse, ErrorResponse>

    @POST("plans/import/all/usb")
    suspend fun importAllSprayPlansFromUsbToNuc(
        @Query("usbname")
        usbName: String
    ): NetworkResponse<SuccessResponse, ErrorResponse>

    companion object {
        operator fun invoke(): ISprayPlansApiService {
            return Retrofit
                .Builder()
                .baseUrl(ApiHostSettings.apiBaseUrl)
                .addCallAdapterFactory(CoroutineCallAdapterFactory())
                .addCallAdapterFactory(NetworkResponseAdapterFactory())
                .addConverterFactory(GsonConverterFactory.create())
                .build()
                .create(ISprayPlansApiService::class.java)
        }
    }
}
