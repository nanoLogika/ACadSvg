# ACad/SVG

ACad/SVG is a library to convert AutoCAD DWG documents to SVG. DWG documents are read with [ACadSharp](https://github.com/DomCR/ACadSharp).

The converter supports many AutoCAD entities such as Arc, Circle, Dimensions, Ellipse, Hatch, Insert, Leader, Line, LwPolyline, MText, Multileader, Spline, TextEntity. The converter focuses on converting the block structure, especially dynamic blocks, rather than just converting a drawing.

## Getting Started
Use the [ACad SVG Studio](https://github.com/nanoLogika/ACadSvgStudio) to load and convert DWG documents and view the converted SVG.

## Code Example



## WIP
The converter does not support all AutoCAD entities. Entitiy types that could not be converted, either because the conversion is not implemented in ACad/SVG or the DWG reader is not implemented in ACadSharp are listed in the conversion log.

Notice that this project is in an alpha version, not all the features are implemented and there can be bugs due to this.

## Contributions
Please feel free to fork this repo and send a pull request if you want to contribute to this project.
