Atreyu
==========

Atreyu is three separate projects centred around viewing data from Pacific Northwest National Laboratory's Unified Ion Mobility Files (UIMF).

The first project, this Repo's namesake **Atreyu**, is a collection of views to visually display the data using Oxyplot to drive the plot manipulation.

  The three plots are a heat map, which is the main control point; the mass over charge or m/z plot, which shows the summed intensity for each m/z currently in view; and the Total Ion Chromatogram or TiC, which shows the summed intensities for each arrival time in view.

  Both the m/z and TiC plots have a simple peak finding algorithm to display information about the peak's resolution.  The TiC plot will show all peaks, while the m/z plot will only show the three highest peaks in view due to the nature of m/z data.

  The second project, **Viewer**, is a thin wrapper around Atreyu in order to be a standalone windows application.  This includes hooks to save the data in view (with the compressions needed to display the data) into csv format.  This also includes options to save an image of the combined graphs.

Finally, the third project is **UimfDataExtractor**.  UimfDataExtractor started as a command line application that was later extended to be used either via CLI or GUI.  When started without any arguments the gui will open.

UimfDataExtractor is designed to do bulk exporting of data from many UIMFs to csv or xml files.  The program will process all UIMFs in a single input directory, or optionally will recursively process through all the sub directories of the input directory.

The user may choose to output csvs for the Heat map, m/z, Tic, and the eXtracted Ion Chromatogram (XiC) for a specific m/z range.  The user may also select to output a xml file with detailed information of every peak in the m/z, TiC, and/or XiC of each file.  

Additionally the user may select to output a bulk comparison file that will have trimmed down details of every peak in all of the files for the axis or axes selected.

Full API documentation and git practices are available in the DevDocs folder.