# ACad/SVG

ACad/SVG is a library to convert AutoCAD DWG documents to SVG. DWG documents are read with [ACadSharp](https://github.com/DomCR/ACadSharp).

The converter supports many AutoCAD entities such as Arc, Circle, Dimensions, Ellipse, Hatch, Insert, Leader, Line, LwPolyline, MText, Multileader, Spline, TextEntity. The converter focuses on converting the block structure, especially dynamic blocks, rather than just converting a drawing.

SVG text is created using [SvgElements](https://github.com/nanoLogika/SvgElements).

## Getting Started
Use the [ACad SVG Studio](https://github.com/nanoLogika/ACadSvgStudio) to load and convert DWG documents and view the converted SVG.

## Code Example
```c#

using ACadSvg;
using SvgElements;

//  Create conversion context assuming standard conversion options.
//  The conversion context also receives the conversion log.
//  See/use ACadSvgStudio to learn more about conversion options.
ConversionContext ctx = new ConversionContext();

string path = "sample.dwg";
DocumentSvg docSvg = ACadLoader.LoadDwg(path, ctx);

//  Get an object representing a SVG group containing the converted
//  entities that are not member of a BlockRecord.
SvgElementBase mainGroup = docSvg.MainGroupToSvgElement();

//  Get an object representing a SVG defs element containing the converted
//  BlockRecord objects found in DWG.
SvgElementBase defs = docSvg.DefsToSvgElement();

//  Create an empty SVG element
SvgElement svg = DocumentSvg.CreateSVG(ctx);

//  Convert the SVG objects to Text
string mainGroupSvg = mainGroup.ToString();
string defsSvg = defs.ToString();

Console.WriteLine(ctx.ConversionInfo.GetLog());
Console.WriteLine(ctx.ConversionInfo.GetOccurringEntitiesLog());
```

## Dependencies
* **SvgElements** https://github.com/nanoLogika/SvgElements
* **ACadSharp** https://github.com/DomCR/ACadSharp, see also forked repo: https://github.com/mme1950/ACadSharp
* **net6.0**

**NOTE:** Currently not all ACadSharp features required by ACadSvg are pushed to [ACadSharp](https://github.com/DomCR/ACadSharp).
If you clone this repository you will get the ACadSharp.dll built from https://github.com/mme1950/ACadSharp.

## WIP
The converter does not support all AutoCAD entities. Entitiy types that could not be converted, either because the conversion is not implemented in ACad/SVG or the DWG reader is not implemented in ACadSharp are listed in the conversion log.

Notice that this project is in an alpha version, not all the features are implemented and there can be bugs due to this.

## Contributions
Please feel free to fork this repo and send a pull request if you want to contribute to this project.
