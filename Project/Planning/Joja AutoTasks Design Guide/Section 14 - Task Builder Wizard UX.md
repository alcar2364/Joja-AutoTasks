# Section 14 — Task Builder Wizard UX #

## 14.1 Purpose ##

The Task Builder Wizard provides a guided interface allowing players to
create custom task rules without directly editing rule definitions.

Task Builder is a core differentiator and must remain on the Version 1
critical path.

The wizard translates player input into deterministic rule definitions
consumed by the rule engine described in Section 7.1. The resulting rules
must produce stable `TaskId` values as defined in Section 3.3.

The wizard must prioritize clarity and error prevention over speed of
entry.

## 14.2 Wizard responsibilities ##

The wizard is responsible for:

    - Guiding the player through rule creation in a structured sequence.
    - Validating rule inputs before rule persistence.
    - Translating UI selections into canonical rule definitions.
    - Ensuring generated rules comply with rule evaluation constraints.
    - Maintaining rule-definition boundaries during Now-stage delivery.

The wizard must not:

    - Directly create or modify tasks.
    - Mutate runtime task state.
    - Bypass the rule persistence pipeline described in Section 9.

The wizard only produces rule definitions that are later evaluated by
the rule engine.

Now-stage constraint:

    - The wizard is limited to rule-definition authoring, validation,
    confirmation, and persistence.
    - The wizard is menu-hosted; the HUD provides launch-to-menu only.
    - Runtime task mutation remains owned by the State Store command path
    after rule evaluation.

## 14.3 Wizard interaction model ##

The wizard follows a multi-step workflow that progressively defines a
rule.

Each step must collect one logical component of the rule definition.

Typical flow:

1. Select rule type.
2. Select trigger condition.
3. Define subject or target entity.
4. Define progress or completion criteria.
5. Define deadline or timing rules.
6. Define metadata and presentation options.
7. Review and confirm rule creation.

The wizard should allow backward navigation to modify earlier steps
before confirmation.

## 14.4 Rule Types ##

Rule types determine the overall behavior model of the rule.

Version 1 supports the following rule types:

    | Rule Type   | Description                                         |
    | ----------- | --------------------------------------------------- |
    | Reminder    | A task triggered by a specific event or date        |
    | Progress    | A task completed after reaching a measurable target |
    | Repeating   | A task that resets according to a schedule          |
    | Milestone   | A long-term achievement or goal                     |

In this document, Rule Type defines engine behavior, while Task Category
defines UI organization labels.

Rule type selection constrains later wizard steps to valid rule options.

## 14.5 Trigger definition ##

Triggers determine when a rule becomes active.

The wizard must provide selectable trigger types corresponding to the
evaluation triggers described in Section 7.8.

Examples include:

    - Day start
    - Inventory change
    - Location change
    - Skill level change
    - Calendar event

Trigger configuration must prevent combinations that would produce
invalid or unreachable rule states.

## 14.6 Subject selection ##

The subject defines the entity that the rule operates on.

Examples include:

    - Specific item
    - Machine type
    - Skill progression
    - Resource count
    - Location activity

The wizard must translate subject selections into deterministic
`SubjectId` values compatible with the identity rules described in
Section 3.9.

## 14.7 Progress definition ##

Progress-based tasks require a measurable completion condition.

The wizard must support defining progress targets such as:

    - Collect N items
    - Perform N actions
    - Reach skill level N
    - Accumulate resource quantity

Progress models must correspond to the progress evaluation models used
by the rule engine described in Section 7.

## 14.8 Deadline and schedule definition ##

Rules may include optional timing constraints.

Supported timing definitions include:

    - Complete by specific in-game date
    - Complete within N days
    - Repeat every N days
    - Repeat each day or week

Timing rules must remain deterministic and compatible with daily
snapshot evaluation described in Section 11.4.

## 14.9 Metadata and presentation ##

The wizard must allow configuration of non-functional metadata used by
the UI.

Examples include:

    - Task display name
    - Task description
    - Category label
    - Icon selection

Metadata must not influence `TaskId` generation unless explicitly
defined as an identity field. See Section 3.3.

## 14.10 Validation rules ##

The wizard must validate rule definitions before persistence.

Validation must ensure:

    - Required fields are populated.
    - Selected triggers are compatible with the rule type.
    - Progress definitions are logically valid.
    - Identity fields are deterministic.

Invalid configurations must prevent rule creation until corrected.

## 14.11 Rule confirmation ##

The final step of the wizard presents a rule summary.

The summary must display:

    - Trigger conditions
    - Subject definition
    - Progress requirements
    - Deadline or schedule
    - Resulting task description

Players must explicitly confirm rule creation before the rule is
persisted.

## 14.12 Rule persistence ##

Upon confirmation, the wizard serializes the rule definition and sends
it to the persistence system described in Section 9.

The rule then becomes part of the rule evaluation pipeline described in
Section 7.9 and may generate tasks during the next evaluation cycle.

The wizard does not write runtime task state directly. Runtime task
entities change only through evaluation and State Store command
application.

## 14.13 Rule template library ##

The wizard should offer a library of pre-built rule templates for common
tasks.

Templates provide a starting point that the player can customize rather
than building a rule from scratch.

Example templates:

    - "Track resource collection" — pre-filled with item trigger, quantity
    target, and progress model
    - "Daily reminder" — pre-filled with day-start trigger and reminder
    metadata
    - "Skill milestone" — pre-filled with skill-level trigger and
    milestone completion
    - "Seasonal calendar event" — pre-filled with date trigger and
    seasonal schedule

Templates are not persisted rules. A template populates the wizard with
default values; the player then modifies and confirms as usual.

New templates may be added in future versions as the rule engine
expands.

## 14.14 Sentence-builder preview ##

The wizard may display a human-readable sentence summarizing the rule as
the player fills in each step.

Example:

-"When I have **300 wood**, mark **Collect Wood** as complete."
-"Every day, remind me to **check crab pots**."

The sentence updates live as the player changes wizard fields. This
helps the player understand the effect of their choices without needing
to interpret the technical rule summary in §14.11.

The sentence is a UI affordance only. It must not influence rule
identity or evaluation behavior.

## 14.15 Rule preview panel ##

Before confirmation, the wizard may show a preview panel demonstrating
how the resulting task would appear in the HUD and menu.

The preview renders a mock task row using the current wizard inputs
(display name, category, icon) so the player can verify
appearance before committing.

The preview is cosmetic and must not create or evaluate any actual task.

## 14.16 Command and snapshot boundary flow ##

Task Builder execution flow must remain boundary-safe.

```text
Wizard interaction -> Rule-definition command -> Rule persistence ->
Rule evaluation engine -> State Store command application -> Snapshot publish
```

In this flow:

    - the wizard authors intent
    - the engine evaluates rules deterministically
    - the State Store owns canonical runtime mutation
    - HUD and Menu render the resulting snapshot
    - no direct runtime task mutation occurs inside wizard steps
