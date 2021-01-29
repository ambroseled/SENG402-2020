package com.doc.wildingpinesui.sidebar

import androidx.fragment.app.Fragment
import com.doc.wildingpinesui.R
import com.doc.wildingpinesui.UnimplementedSectionFragment
import com.doc.wildingpinesui.modules.home.HomeFragment
import com.doc.wildingpinesui.modules.setplan.SetPlanFragment
import com.doc.wildingpinesui.modules.syncfiles.SyncFilesFragment
import com.doc.wildingpinesui.modules.testnozzles.TestNozzlesFragment
import java.util.*

/**
 * Defines the sections in the sidebar.
 */
object SidebarSections {
    val sections: MutableList<SidebarSection> = ArrayList()

    /**
     * Used to distinguish between sections in the sidebar.
     * @param displayName the name displayed in the UI for the section.
     */
    enum class SectionName(val displayName: String) {
        HOME("Home"),
        SET_PLAN("Set plan"),
        SYNC_FILES("Sync files"),
        TEST_NOZZLES("Test nozzles"),
        SEE_LOGS("See logs"),
        SETTINGS("Settings")
    }

    init {
        addSection(SidebarSection(R.drawable.ic_home_24dp, SectionName.HOME))
        addSection(SidebarSection(R.drawable.ic_map_24dp, SectionName.SET_PLAN))
        addSection(SidebarSection(R.drawable.ic_sync_24dp, SectionName.SYNC_FILES))
        addSection(SidebarSection(R.drawable.ic_check_24dp, SectionName.TEST_NOZZLES))
        addSection(SidebarSection(R.drawable.ic_clipboard_24dp, SectionName.SEE_LOGS))
        addSection(SidebarSection(R.drawable.ic_settings_24dp, SectionName.SETTINGS))
    }

    /**
     * Add a section of the app to the sidebar.
     */
    private fun addSection(sidebarSection: SidebarSection) {
        sections.add(sidebarSection)
    }

    /**
     * Represents a section in the sidebar.
     */
    class SidebarSection(val iconId: Int, val sectionName: SectionName) {
        fun buildFragment(): Fragment {
            return when (sectionName) {
                SectionName.SYNC_FILES -> SyncFilesFragment()
                SectionName.SET_PLAN -> SetPlanFragment()
                SectionName.TEST_NOZZLES -> TestNozzlesFragment()
                else -> UnimplementedSectionFragment()
            }
        }
    }
}
