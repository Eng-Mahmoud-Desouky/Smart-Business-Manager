---
name: Intelligent Commerce
colors:
  surface: '#fcf8ff'
  surface-dim: '#dcd8e5'
  surface-bright: '#fcf8ff'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f5f2ff'
  surface-container: '#f0ecf9'
  surface-container-high: '#eae6f4'
  surface-container-highest: '#e4e1ee'
  on-surface: '#1b1b24'
  on-surface-variant: '#464555'
  inverse-surface: '#302f39'
  inverse-on-surface: '#f3effc'
  outline: '#777587'
  outline-variant: '#c7c4d8'
  surface-tint: '#4d44e3'
  primary: '#3525cd'
  on-primary: '#ffffff'
  primary-container: '#4f46e5'
  on-primary-container: '#dad7ff'
  inverse-primary: '#c3c0ff'
  secondary: '#006c49'
  on-secondary: '#ffffff'
  secondary-container: '#6cf8bb'
  on-secondary-container: '#00714d'
  tertiary: '#7e3000'
  on-tertiary: '#ffffff'
  tertiary-container: '#a44100'
  on-tertiary-container: '#ffd2be'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#e2dfff'
  primary-fixed-dim: '#c3c0ff'
  on-primary-fixed: '#0f0069'
  on-primary-fixed-variant: '#3323cc'
  secondary-fixed: '#6ffbbe'
  secondary-fixed-dim: '#4edea3'
  on-secondary-fixed: '#002113'
  on-secondary-fixed-variant: '#005236'
  tertiary-fixed: '#ffdbcc'
  tertiary-fixed-dim: '#ffb695'
  on-tertiary-fixed: '#351000'
  on-tertiary-fixed-variant: '#7b2f00'
  background: '#fcf8ff'
  on-background: '#1b1b24'
  surface-variant: '#e4e1ee'
typography:
  h1:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
    letterSpacing: -0.02em
  h2:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
    letterSpacing: -0.01em
  h3:
    fontFamily: Inter
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-lg:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-sm:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  label-caps:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
    letterSpacing: 0.05em
  insight-text:
    fontFamily: Inter
    fontSize: 15px
    fontWeight: '500'
    lineHeight: 22px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  unit: 4px
  container_margin: 16px
  gutter: 12px
  stack_sm: 8px
  stack_md: 16px
  stack_lg: 24px
---

## Brand & Style

This design system is built for "Smart Business Manager," a high-utility mobile tool designed to instill confidence and clarity in business owners. The aesthetic is rooted in **Modern Minimalism**, specifically drawing from the "Linear-Stripe" lineage: it prioritizes precision, mathematical balance, and a sense of effortless intelligence.

The brand personality is **Professional, Insightful, and Frictionless**. The UI should evoke a feeling of "organized power"—where complex data feels manageable and automated insights feel like a premium concierge service. To achieve this, the system utilizes significant white space, ultra-refined typography, and subtle motion to guide the user's eye toward critical actions.

## Colors

The palette is anchored by **Deep Indigo**, providing a foundation of stability and institutional trust. Functional colors (Emerald, Amber, Rose) are used strictly for status signaling to maintain high signal-to-noise ratios.

The "Magic" color profile is reserved exclusively for AI-driven features. It utilizes a soft Indigo-to-Violet gradient. When applied to "Smart Insight" cards, it should be accompanied by a 10% opacity background tint or a 2px glowing outer stroke to distinguish it from standard manual data. Use a light mode default to maximize readability and the "clean" SaaS aesthetic.

## Typography

This design system uses **Inter** for its neutral, highly legible character. The hierarchy is "top-heavy," meaning headings are significantly bolder and tighter in letter-spacing than body text to create clear visual anchors on a small mobile screen.

For AI-generated content or "Smart Insights," use the `insight-text` style to provide a subtle stylistic shift from user-entered data. Ensure numerical data in financial tables uses tabular lining figures to maintain vertical alignment in lists.

## Layout & Spacing

This design system utilizes a **Fluid Grid** optimized for mobile viewports. Content is primarily organized in a single-column stack of cards. 

The spacing rhythm is based on a **4px baseline grid**. Standard page margins are set to 16px to allow the large-radius cards to feel "nested" without touching the screen edges. Use generous vertical padding (24px+) between distinct sections to prevent the UI from feeling cluttered, adhering to the minimalist philosophy.

## Elevation & Depth

Depth is achieved through **Ambient Shadows** and **Tonal Layers**. Instead of heavy black shadows, use multi-layered, highly diffused shadows with a tiny hint of the Primary Indigo color (#4F46E5) at 4-8% opacity.

- **Level 0 (Background):** #F9FAFB (Flat)
- **Level 1 (Cards/Sheet):** White background, 1px subtle border (#E5E7EB), and a soft "Soft-Low" shadow.
- **Level 2 (Modals/Popovers):** White background, "Soft-High" shadow for significant lift.

For "Smart Insight" cards, add a 4px blur glow in the AI gradient color behind the card to create a "magical" hovering effect.

## Shapes

The shape language is defined by **Large Border Radii**, creating a friendly yet sophisticated silhouette. 

- **Primary Containers (Cards):** 16px radius.
- **Secondary Elements (Buttons, Inputs):** 12px radius.
- **Small Elements (Chips, Tags):** 8px or fully rounded (pill) depending on context.

This consistent use of "squircle-like" curves softens the professional Indigo palette, making the app feel modern and approachable.

## Components

### Buttons & CTAs
Buttons feature a subtle top-light inner gradient to mimic a tactile feel. Primary buttons use the Deep Indigo background with white text. Ghost buttons use a 1px border and 500-weight text.

### Smart Insight Cards
These are the signature components. They feature a 2px left-border using the AI Magic Gradient. The background should be a very faint Lavender/Indigo tint (2% opacity). Include a "sparkle" icon in the top right to signal AI generation.

### Inputs & Fields
Inputs use a 12px radius and a 1px #E5E7EB border. On focus, the border transitions to Primary Indigo with a 3px soft outer glow (the "ring" effect).

### Lists & Data Rows
List items are separated by subtle horizontal dividers (#F3F4F6) or housed within individual cards. Every row should have a clear "chevron-right" or "action" icon if it is tappable.

### Progress & Status
Use pill-shaped chips for status. "Success" uses Emerald Green text on a 10% opacity Emerald background. This "Soft-Label" pattern applies to Warning and Danger states as well.


