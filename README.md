# XamlToWebViewApp
# XAML to HTML Converter (WPF)

## Overview

This project implements a scalable conversion pipeline that transforms XAML UI definitions into HTML and renders the output inside a WebView2 control.

The system is designed with modular architecture to support future extension of additional XAML components without modifying core logic.

---

## Architecture Overview

The conversion follows a layered pipeline:

XAML → XElement → IR → Renderers → HTML → WebView2

### 1. Parsing Layer
- XamlParser
- Uses LINQ to XML to load and parse XAML files into XElement.

### 2. Intermediate Representation (IR)
- IrElement
- IrBuilder
- Converts parsed XML into a neutral UI model independent of output format.

### 3. Rendering Engine
- IElementRenderer
- RendererFactory
- RendererAttribute
- Component-specific renderers:
  - ButtonRenderer
  - TextBlockRenderer
  - StackPanelRenderer

Renderers are automatically discovered using attribute-based registration (reflection), enabling plugin-style extensibility.

### 4. HTML Generation
- `HtmlGenerator`
- Delegates rendering responsibility to component renderers.
- Wraps output inside a complete HTML document.

### 5. Preview Layer
- WebView2 used to render generated HTML.
- Uses file-based navigation for stable rendering.

---

## Key Features

- Modular component-based rendering
- Automatic renderer registration
- Scalable architecture
- Separation of concerns (Parsing / IR / Rendering / UI)
- Unit tested rendering modules
- Stable WebView2 preview implementation

---

## Running Tests

Run tests using:

```bash
dotnet test
