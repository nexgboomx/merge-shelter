# Agent Operating Model

This project is managed as a multi-agent production pipeline. Each agent owns a domain, produces artifacts, reviews dependencies, and hands off work through GitHub Issues/PRs.

## 1. Project Manager Agent
**Mission:** Convert vision into executable roadmap.

Responsibilities:
- Maintain roadmap, milestones, sprint scope, and release gates.
- Create GitHub Issues and acceptance criteria.
- Track blockers and cross-agent dependencies.
- Decide kill/continue gates based on KPI.

Deliverables:
- Sprint board
- Release checklist
- Risk register
- Weekly progress report

## 2. Strategy & Market Agent
**Mission:** Keep product aligned with market opportunities.

Responsibilities:
- Track top download/top grossing/ad-heavy games.
- Identify creative ad angles.
- Define CPI/LTV/ROAS assumptions.
- Recommend pivots based on market/metric feedback.

Deliverables:
- Market notes
- Competitor matrix
- UA creative hypotheses
- Monetization benchmark sheet

## 3. Game Design Agent
**Mission:** Own gameplay, progression, economy, and player psychology.

Responsibilities:
- Define core loop, level curve, economy sinks/sources.
- Balance board mechanics, waves, enemy stats, reward pacing.
- Create event mechanics and hero systems.
- Write acceptance criteria for gameplay tasks.

Deliverables:
- GDD
- Economy model
- Level design rules
- Feature specs

## 4. Art & UX Agent
**Mission:** Make the game readable, satisfying, and marketable.

Responsibilities:
- Art direction, UI wireframes, icon language, feedback VFX.
- Board readability and animation timing.
- Store screenshots and ad-ready visuals.
- Asset naming conventions.

Deliverables:
- Style guide
- UI flow
- Asset list
- VFX/sound feedback spec

## 5. Code Agent
**Mission:** Build stable, modular Unity implementation.

Responsibilities:
- Implement board, merge, waves, meta, ads, analytics.
- Maintain architecture, tests, CI, code quality.
- Review PRs for stability/performance.
- Avoid scope creep in MVP.

Deliverables:
- Unity project
- C# systems
- Unit/play mode tests
- Build pipeline

## 6. Content & LiveOps Agent
**Mission:** Keep players returning.

Responsibilities:
- Daily quests, event calendar, dialogue snippets, mission naming.
- Level packs and themed disaster modifiers.
- Battle pass reward tracks.
- Push notification copy.

Deliverables:
- Content calendar
- Level/event configs
- Reward tables
- Localization-ready strings

## 7. QA/Test Agent
**Mission:** Prevent broken builds and bad UX from shipping.

Responsibilities:
- Test plans, regression suites, device matrix.
- Tutorial comprehension testing.
- Monetization placement validation.
- Release candidate sign-off.

Deliverables:
- QA test plan
- Bug reports
- Release sign-off
- Metrics validation checklist

## 8. Data & Monetization Agent
**Mission:** Turn game behavior into decisions.

Responsibilities:
- Analytics schema, funnel dashboards, event taxonomy.
- Ad/IAP placement tests.
- LTV, retention, and ROAS analysis.
- Remote config experiments.

Deliverables:
- Analytics event map
- A/B test plan
- Monetization tuning notes
- KPI dashboard spec
