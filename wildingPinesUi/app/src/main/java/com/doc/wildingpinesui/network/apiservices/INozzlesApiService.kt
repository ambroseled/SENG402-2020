package com.doc.wildingpinesui.network.apiservices

import com.doc.wildingpinesui.model.ErrorResponse
import com.doc.wildingpinesui.model.Nozzle
import com.doc.wildingpinesui.model.SuccessResponse
import com.haroldadmin.cnradapter.NetworkResponse
import com.haroldadmin.cnradapter.NetworkResponseAdapterFactory
import com.jakewharton.retrofit2.adapter.kotlin.coroutines.CoroutineCallAdapterFactory
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.PATCH
import retrofit2.http.POST

/**
 * Used to make API calls to get the number of nozzles, and testing them from the app.
 */
interface INozzlesApiService {

    /**
     * Gets the nozzles available in the spray boom.
     * NOTE: the values for the shouldBeSpraying variable are not meaningful here.
     */
    @GET("nozzles")
    suspend fun getNozzles(): NetworkResponse<List<Nozzle>, ErrorResponse>

    /**
     * @param nozzles the nozzles with their desired status.
     */
    @PATCH("nozzles")
    suspend fun updateNozzleStatuses(@Body nozzles: List<Nozzle>): NetworkResponse<SuccessResponse, ErrorResponse>

    companion object {
        operator fun invoke(): INozzlesApiService {
            return Retrofit
                .Builder()
                .baseUrl(ApiHostSettings.apiBaseUrl)
                .addCallAdapterFactory(CoroutineCallAdapterFactory())
                .addCallAdapterFactory(NetworkResponseAdapterFactory())
                .addConverterFactory(GsonConverterFactory.create())
                .build()
                .create(INozzlesApiService::class.java)
        }
    }
}