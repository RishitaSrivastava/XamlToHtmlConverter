# XamlToHtmlConverter Gap Analysis & Implementation Plan

**Date**: March 23, 2026  
**Branch**: feature/GapFix2  
**Status**: Planning Phase for 9 Critical Gaps (1 HIGH, 7 MEDIUM, 1 LOW)

---

## Executive Summary

This gap analysis identifies **9 architectural and functional gaps** in the XAML-to-HTML converter that cause silent failures, missing features, and incomplete mappings. The issues span property mappers, event extraction, behavior handlers, layout renderers, and runtime functionality.

| Gap | Severity | Category | Impact |
|-----|----------|----------|--------|
| G10 | 🔴 **HIGH** | Property Parser | BorderThickness/CornerRadius multi-values silently dropped |
| G11 | 🟡 **MEDIUM** | Property Mappers | 9 common XAML properties have no CSS mappers |
| G12 | 🟡 **MEDIUM** | Event Handling | 9 XAML events missing from pipeline |
| G13 | 🟡 **MEDIUM** | Bindings | 8 binding types unhandled by BehaviorRegistry |
| G14 | 🟡 **MEDIUM** | Layout | Canvas absolute positioning not implemented |
| G15 | 🟡 **MEDIUM** | Layout | DockPanel mixed-axis layout incomplete |
| G16 | 🟡 **MEDIUM** | Property Parser | "Auto" and "*" size values silently ignored |
| G17 | 🟡 **MEDIUM** | Runtime | xaml-runtime.js missing (all bindings non-functional) |
| G18 | 🟢 **LOW** | Code Quality | Debug Console.WriteLine left in production |
| **TOTAL** | - | - | **9 improvements needed** |

---

## Gap Details & Implementation Plan

### G10: 🔴 BorderThickness and CornerRadius Multi-Value Parsing

**Problem:**
```xaml
BorderThickness="1,0,1,0"  <!-- left, top, right, bottom -->
CornerRadius="5,5,0,0"     <!-- top-left, top-right, bottom-right, bottom-left -->
```
Current code only handles single integer values. Multi-value strings fail silently, producing no CSS output.

**Location**: `BorderMapper.cs`

**Fix Implementation**:
- Parse comma-separated values
- Map XAML order (left, top, right, bottom) → CSS order (top, right, bottom, left)
- Handle both `BorderThickness="2"` and `BorderThickness="1,0,1,0"` formats

**Estimated Effort**: 30 minutes | **Priority**: HIGH

---

### G11: 🟡 Missing CSS Property Mappers

**Problem**: 9 commonly-used XAML properties lack any CSS mapper:

| Property | Expected CSS | Current Status |
|----------|--------------|-----------------|
| `Opacity` | `opacity: value` | ❌ None |
| `FontStyle` | `font-style: italic` | ❌ None |
| `TextDecorations` | `text-decoration: underline` | ❌ None |
| `TextWrapping` | `white-space: wrap` | ❌ None |
| `TextTrimming` | `text-overflow: ellipsis` | ❌ None |
| `LineHeight` | `line-height: {value}px` | ❌ None |
| `FlowDirection` | `direction: rtl` | ❌ None |
| `Cursor` | `cursor: pointer` | ❌ None |
| `ClipToBounds` | `overflow: hidden` | ❌ None |

**Location**: `TypographyMapper.cs` (extend) + new `MiscPropertyMapper.cs`

**Fix Implementation**:
1. Extend `TypographyMapper.cs` CanHandle() with FontStyle, TextDecorations, TextWrapping, TextTrimming, LineHeight
2. Create new `MiscPropertyMapper.cs` for Opacity, FlowDirection, Cursor, ClipToBounds, IsEnabled
3. Register both mappers in `DefaultStyleBuilder` factory
4. Handle ToolTip as HTML title attribute in HtmlRenderer

**Estimated Effort**: 60 minutes | **Priority**: MEDIUM

---

### G12: 🟡 Missing Events in DefaultEventExtractor

**Problem**: Only 6 events recognized; 9 common ones missing:

**Missing Events**:
- Mouse events: MouseEnter, MouseLeave, MouseDown, MouseUp
- Keyboard events: KeyDown, KeyUp
- Focus events: GotFocus, LostFocus
- Component events: Unloaded, ValueChanged, DropDownOpened, DropDownClosed, DragEnter, DragLeave, Drop

**Location**: `DefaultEventExtractor.cs` (v_KnownEvents HashSet)

**Fix Implementation**:
- Add all missing event names to `v_KnownEvents`
- Recover MouseEnter/MouseLeave from dead `BehaviorExtractor` class
- No other code changes needed (Extract() already iterates and emits data-event-*)

**Estimated Effort**: 15 minutes | **Priority**: MEDIUM

---

### G13: 🟡 Missing Binding Behaviors in BehaviorRegistry

**Problem**: 8 binding types not handled by any IBehaviorHandler:

**Missing Bindings**:
- Selection: SelectedIndex, SelectedItem, SelectedValue
- Input: IsReadOnly, Value
- State: IsExpanded, Header
- Security: Password

**Location**: `BehaviorRegistry.cs` + new behavior handlers

**Fix Implementation**:
1. Create 6 new IBehaviorHandler classes:
   - `SelectionBehavior.cs` (SelectedIndex, SelectedItem, SelectedValue)
   - `ValueBehavior.cs` (Value)
   - `IsReadOnlyBehavior.cs` (IsReadOnly with fallback to readonly attribute)
   - `IsExpandedBehavior.cs` (IsExpanded)
   - `HeaderBehavior.cs` (Header with static fallback)
   - `PasswordBehavior.cs` (Password)
2. Register all handlers in `HtmlRendererFactory.Create()`
3. Use shared `BindingExpressionHelper` to extract binding paths

**Estimated Effort**: 75 minutes | **Priority**: MEDIUM

---

### G14: 🟡 Canvas Layout Not Handled

**Problem**: No CanvasLayoutRenderer; Canvas.Left/Top/Right/Bottom attached properties ignored. Children stack on top of each other.

**Location**: New `CanvasLayoutRenderer.cs` + `DefaultStyleBuilder.ApplyAttachedProperties()`

**Fix Implementation**:
1. Create `CanvasLayoutRenderer` with `position:relative;overflow:hidden;`
2. In `DefaultStyleBuilder`, detect and apply Canvas.Left, Canvas.Top, Canvas.Right, Canvas.Bottom as `position:absolute;`
3. Register in layout renderer collection

**Estimated Effort**: 30 minutes | **Priority**: MEDIUM

---

### G15: 🟡 DockPanel Layout Architecturally Incomplete

**Problem**: Current single flex-direction cannot handle mixed-axis docked children (Top, Left, Fill, Right, Bottom all in one container).

**Root Cause**: WPF DockPanel supports arbitrary axis-mixing; CSS flexbox with single direction cannot.

**Location**: `DockPanelLayoutRenderer.cs`

**Fix Implementation**:
1. Replace flat flex with **two nested flex containers**:
   - Outer vertical flex: Top → [inner row] → Bottom
   - Inner horizontal flex: Left → Fill → Right
2. Add architectural note documenting the limitation
3. This is a best-effort approximation; document that Grid-based layouts may be more accurate for complex UIs

**Estimated Effort**: 45 minutes | **Priority**: MEDIUM

---

### G16: 🟡 Width="Auto" and Height="Auto" Silently Dropped

**Problem**: 
```xaml
<Button Width="Auto" Height="Auto" />   <!-- Produces no CSS output -->
<Button Width="*" Height="*" />         <!-- Also produces no CSS output -->
```

Current mappers: `if (int.TryParse(...))` → silent failure for non-numeric values.

**Location**: `WidthMapper.cs`, `HeightMapper.cs`

**Fix Implementation**:
1. Before int.TryParse(), check for "Auto" → `width:auto;`
2. Check for "*" → `width:100%;`
3. Fall through to int.TryParse() for pixel values

**Estimated Effort**: 20 minutes | **Priority**: MEDIUM

---

### G17: 🟡 xaml-runtime.js Missing (Critical for MVVM)

**Problem**: 
```html
<script src="xaml-runtime.js"></script>  <!-- File doesn't exist -->
```

All `data-binding-*`, `data-event-*`, `data-trigger-*`, `data-command=*` attributes the converter emits are meaningless without a JavaScript runtime to interpret them. **The HTML output is non-functional scaffolding.**

**Location**: New `xaml-runtime.js` file + EmbeddedResource registration

**Fix Implementation**:
1. Create `xaml-runtime.js` with `XamlRuntime` global providing:
   - `bind(rootEl, viewModel)` - Wire data-binding-* to ViewModel properties
   - Two-way binding for input controls (change/input events)
   - Event handler wiring (data-event-* → DOM events)
   - Command binding support (data-command with canExecute/execute)
2. Add EmbeddedResource to `.csproj`
3. This is the **MVVM bridge** that makes the converter output functional

**Estimated Effort**: 90 minutes | **Priority**: MEDIUM (high business impact)

---

### G18: 🟢 Stray Console.WriteLine in TriggerEngine

**Problem**:
```csharp
Console.WriteLine($"Triggers found: {element.Triggers.Count}");  // Debug statement left in
```

**Location**: `TriggerEngine.cs`, Extract() method

**Fix Implementation** (choose one):
- **Option A** (Simple): Just delete the line
- **Option B** (Better): Replace with ILogger.LogDebug() injection

**Estimated Effort**: 5 minutes | **Priority**: LOW

---

## Implementation Strategy

### Phase 1: High-Impact Fixes (Priority: MEDIUM+)
1. **G16** (WidthMapper/HeightMapper) - 20 min - Quick win
2. **G12** (DefaultEventExtractor) - 15 min - Quick fix
3. **G10** (BorderThickness) - 30 min - High severity
4. **G14** (CanvasLayoutRenderer) - 30 min - New layout support

### Phase 2: Comprehensive Mappers
5. **G11** (CSS Mappers) - 60 min - Broad coverage
6. **G13** (Binding Behaviors) - 75 min - Binding support

### Phase 3: Architecture & Runtime
7. **G15** (DockPanel Refactor) - 45 min - Complex layout
8. **G17** (xaml-runtime.js) - 90 min - Makes bindings functional
9. **G18** (Console.WriteLine) - 5 min - Cleanup

### Estimated Total Effort: ~370 minutes (6.2 hours)

---

## Testing Strategy

After each gap fix:
1. Build solution (zero errors)
2. Run unit tests (all 155 pass)
3. Run main app (correct conversion)
4. For G17: Create test HTML with viewModel and verify two-way binding works

---

## Commit Strategy

Create separate commits per gap or group related gaps:
```
fix(G10): Support multi-value BorderThickness and CornerRadius
fix(G12,G16): Add missing events and Auto/Star sizing support
feat(G11): Implement CSS mappers for Opacity, FontStyle, TextWrapping...
feat(G13): Add binding behaviors for Selection, Value, IsReadOnly, etc.
feat(G14,G15): Implement Canvas and improve DockPanel layouts
feat(G17): Create xaml-runtime.js MVVM bridge
chore(G18): Remove debug Console.WriteLine from TriggerEngine
```

---

## Success Criteria

✅ All 9 gaps closed with code implementations  
✅ Build succeeds with zero errors  
✅ All 155 unit tests pass  
✅ Main application runs and converts XAML correctly  
✅ No functional regressions  
✅ xaml-runtime.js tested with sample ViewModel binding  

---

## Next Steps

Ready to implement these gaps systematically on the `feature/improvements` branch. 

**Would you like me to:**
1. Start with Phase 1 (quick wins: G16, G12, G10, G14)?
2. Implement all gaps in priority order?
3. Focus on a specific gap first?

Let me know which priority works best for your team! 🚀
