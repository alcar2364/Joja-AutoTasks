# Tech Plan: [Epic Name]

## Architectural Approach

[3–5 key architectural decisions that shape this implementation]

**Decision 1: [name]**
Rationale: ...
Trade-offs: ...

**Decision 2: [name]**
Rationale: ...
Trade-offs: ...

**Constraints:** [technical, business, or regulatory constraints bounding the solution]

---

## Data Model

[New entities and schema changes only. Relationships with existing models.]

```sql
-- Example: new table
CREATE TABLE example (
  id UUID PRIMARY KEY,
  ...
);
```

[Relationships with existing data models and any migration notes]

---

## Component Architecture

[New components required, their responsibilities, and how they connect to existing components]

**New Component: [name]**
- Responsibility: ...
- Interface with existing: ...
- Data flow: ...

**Integration Points:**
- [Existing component A] ↔ [New component]: [how they interact]
- [Existing component B] ↔ [New component]: [how they interact]
