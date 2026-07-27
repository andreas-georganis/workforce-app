---
description: 'Resource naming convention for application types and contracts'
applyTo: '**/*.cs, **/*.razor, **/*.razor.cs, **/*.ts, **/*.tsx'
---

# Resource Naming

## Core Rule
- Use the plain resource noun as the type name by default (for example, `Employee`, `Skill`, `Project`).
- Do not add extra context when there is no naming conflict.
- Default to one type per resource per bounded context.
- Reuse the same type for the same resource/concept across nearby layers when shape and validation needs are compatible.

## Consolidation First
- Avoid creating parallel types for the same concept (for example separate UI/API/domain models with identical fields) unless there is a concrete boundary difference.
- Before introducing a new type, check whether the existing resource type can be extended safely (for example with validation attributes or optional members).
- Prefer consolidating duplicate models into a single resource type and remove obsolete aliases.
- Create a separate type only when at least one of these is true:
	- The boundary has different invariants or lifecycle rules.
	- The shape differs meaningfully (not just naming).
	- Security, serialization, or persistence concerns require separation.

## When Suffixes Are Allowed
- Add a suffix only when there is a real naming conflict in the same scope, or when it communicates a real boundary or role that cannot be expressed otherwise.
- Good examples: `EmployeeEntity` (persistence), `CreateEmployeeCommand` (application command), `EmployeeFormState` (UI state), `EmployeeApiContract` (explicit API boundary).

## Avoid
- Avoid naming that adds no domain meaning: `EmployeeDto`, `EmployeeModel`, `EmployeeRequest`, `EmployeeResponse`.

## Conflict Handling
- If two types would otherwise have the same name in the same scope, keep the resource noun for the primary type and add the minimum meaningful qualifier to the other type.
- Prefer conflict-driven qualifiers over generic suffixes.