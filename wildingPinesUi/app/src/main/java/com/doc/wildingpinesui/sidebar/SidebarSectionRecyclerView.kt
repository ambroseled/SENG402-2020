package com.doc.wildingpinesui.sidebar

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.doc.wildingpinesui.MainActivity
import com.doc.wildingpinesui.R
import kotlinx.android.synthetic.main.sidebar_section_item.view.*

/**
 * Used to contain the different sections in the sidebar.
 * Handles replacing the section of the app shown when a sidebar item is clicked.
 */
class SidebarSectionRecyclerView(
    private val parentActivity: MainActivity,
    private val values: List<SidebarSections.SidebarSection>
) :
    RecyclerView.Adapter<SidebarSectionRecyclerView.ViewHolder>() {

    private val onClickListener: View.OnClickListener

    init {
        onClickListener = View.OnClickListener { v ->
            // when an item is tapped, swap the fragment for the details view
            val sidebarSection = v.tag as SidebarSections.SidebarSection
            updateFragment(sidebarSection)
        }
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.sidebar_section_item, parent, false)
        return ViewHolder(view)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val sidebarSection = values[position]
        holder.itemIcon.setImageResource(sidebarSection.iconId)
        holder.contentView.text = sidebarSection.sectionName.displayName

        with(holder.itemView) {
            tag = sidebarSection
            setOnClickListener(onClickListener)
        }
    }

    override fun getItemCount() = values.size

    /**
     * Set the fragment to be the home when the recycler view attaches.
     */
    override fun onAttachedToRecyclerView(recyclerView: RecyclerView) {
        super.onAttachedToRecyclerView(recyclerView)
        val testNozzles = values[3]
        updateFragment(testNozzles)
    }

    /**
     * Given a sidebar section, update the fragment shown to show that section of the app.
     */
    private fun updateFragment(sidebarSection: SidebarSections.SidebarSection) {
        val fragment = sidebarSection.buildFragment()
        parentActivity.supportFragmentManager
            .beginTransaction()
            .replace(R.id.current_section_container, fragment)
            .commit()
    }

    /**
     * Hold references to the section icon and name.
     * Used by the [SidebarSectionRecyclerView].
     */
    inner class ViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        val itemIcon: ImageView = view.item_icon
        val contentView: TextView = view.content
    }
}