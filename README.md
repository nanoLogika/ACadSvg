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
* **ACadSharp (local assembly 3.7.16)** https://github.com/DomCR/ACadSharp
* **net8.0**

## WIP
The converter does not support all AutoCAD entities. Entitiy types that could not be converted, either because the conversion is not implemented in ACad/SVG or the DWG reader is not implemented in ACadSharp are listed in the conversion log.

Notice that this project is in an alpha version, not all the features are implemented and there can be bugs due to this.

## Contributions
Please feel free to fork this repo and send a pull request if you want to contribute to this project.

## ACadSharp 3.7 migration

Validated against the existing Imports/ACadSharp.dll (file version 3.7.16.0).
The published NuGet release at migration time is 3.7.1; this project continues
to use the supplied local DLL rather than replacing it with the NuGet package.

API adaptations:
* Import CSMath.Extensions for vector operations.
* Iterate BlockVisibilityParameter.States.Values.
* Use Hatch.BoundaryPath.Ellipse.RadiusRatio.
* Translate AngularZeroHandling values 0/1/2/3 to the decimal formatter's
  ZeroHandling values 0/4/8/12 after reading dimension-style overrides.

ACadSvgStudio references the ACadSvg project directly so builds use the updated
converter without manually copying ACadSvg.dll.

Validation:
* Build the containing ACadSvgSuite.sln.
* Run ACadSvg.Test with filter FullyQualifiedName~ACadSharpMigrationTests for
  decimal angular formatting and dynamic visibility group regression coverage.
* The full existing MText suite currently reports failures in TestFont,
  TestSubSpanFont and TestSubSpanOverstrike. Those cases have not been changed
  as part of this migration.
* Representative production DWG files still need a visual conversion check.