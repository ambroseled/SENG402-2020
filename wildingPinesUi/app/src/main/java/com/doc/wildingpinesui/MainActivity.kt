package com.doc.wildingpinesui

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.Observer
import androidx.lifecycle.lifecycleScope
import androidx.recyclerview.widget.RecyclerView
import com.doc.wildingpinesui.model.SystemStatus
import com.doc.wildingpinesui.sidebar.SidebarSectionRecyclerView
import com.doc.wildingpinesui.sidebar.SidebarSections
import com.google.android.material.snackbar.Snackbar
import kotlinx.android.synthetic.main.sidebar_and_section_fragment.*
import kotlinx.coroutines.launch
import org.kodein.di.KodeinAware
import org.kodein.di.android.closestKodein
import org.kodein.di.generic.instance
import java.text.SimpleDateFormat

/**
 * The main activity of the app.
 */
class MainActivity : AppCompatActivity(), KodeinAware {
    override val kodein by closestKodein()
    private val viewModel: MainActivityViewModel by instance()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.main_activity)
        setupRecyclerView(sections_list)
        bindUI()
    }

    /**
     * Set up the recycler view for the sidebar with the app sections.
     */
    private fun setupRecyclerView(recyclerView: RecyclerView) {
        recyclerView.adapter = SidebarSectionRecyclerView(this, SidebarSections.sections)
    }

    /**
     * Populate the UI and define the logic to handle user activity.
     */
    private fun bindUI() {
        setUpLogicForShutdownButton()

        watchSystemStatus()
        watchLastStatusUpdate()
    }

    /**
     * Update the status indicator when the system status is refreshed.
     */
    private fun watchSystemStatus() {
        viewModel.systemStatus.observe(this, Observer {
            updateSystemStatusDot(it)
        })
    }

    /**
     * Update the last sync time in the UI when it is refreshed in the [MainActivityViewModel].
     */
    private fun watchLastStatusUpdate() {
        viewModel.lastStatusUpdate.observe(this, Observer {
            val statusText = when(it) {
                null -> "Haven't synced yet. Trying now..."
                else -> {
                    val formatter = SimpleDateFormat("dd/MM/yyyy 'at' HH:mm")
                    "Last sync on ${formatter.format(it)}"
                }
            }
            statusTextView.text = statusText
        })
    }

    /**
     * When we request the NUC to shut down, tell the user whether the NUC acknowledged the request.
     */
    private fun setUpLogicForShutdownButton() {
        shutdownButton.setOnClickListener {
            lifecycleScope.launch {
                val shutdownRequested = viewModel.requestNucShutdown()
                val message = if (shutdownRequested) "Requested shutdown from NUC" else
                    "Something went wrong trying to shut down the NUC. Are you connected?"
                Snackbar.make(it, message, Snackbar.LENGTH_SHORT).show()
            }
        }
    }

    /**
     * Update the UI dot to change colours according to the new system status.
     */
    private fun updateSystemStatusDot(status: SystemStatus) {
        val color = when (status) {
            SystemStatus.OK -> getColor(R.color.greenAccent)
            SystemStatus.NUC_UNREACHABLE -> getColor(R.color.redAccent)
        }
        statusCircleImageView.setColorFilter(color)
    }
}
