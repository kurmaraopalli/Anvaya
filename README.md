# 🌍 Anvaya: Open-Source, Headless Global Loyalty & Membership API Engine

**Anvaya** (derived from the ancient 5,000-year-old Sanskrit word for *"Connection"* or *"Linkage"*) is a high-performance, developer-first, event-driven loyalty and membership platform built on **.NET Core**. 

It is architected using **completely abstract business entities**, allowing businesses from **any industry vertical**—E-commerce, Dining, Aviation, SaaS, or Fintech—to plug in their transaction pipelines and deploy standard or premium customer retention mechanics globally.

---

## ✨ Enterprise-Grade Features Built In

*   **Universal Event Ingestion Layer:** No rigid schemas. Pass arbitrary metadata keys (e.g., `SKU`, `CabinClass`, `StoreLocation`) to evaluate custom reward scripts.
*   **Cross-Border Exchange Matrix:** Process transactions in localized fiat (e.g., `INR`, `EUR`) while the engine auto-converts, tracks financials, and mints values in the tenant’s `BaseCurrency`.
*   **Unified Timezone Isolation:** Avoid campaign boundaries breaking across timelines. Transactions are saved in strictly enforced `UTC`, while business rules execute in the tenant's native IANA timezone.
*   **Subscription Membership Framework:** Wire premium paid access options directly into loyalty pathways, unlocking multipliers and instant milestone perks.
*   **Household Balance Pooling:** Connect multiple user IDs into a single shared ledger ecosystem for community and family-based challenges.

---

## 🛠️ Local Environment Orchestration (DevEnv / Nix)

This repository includes full **DevEnv** integration configurations. You do not need to pre-install the .NET SDK or runtime dependencies manually. 

### Quick Start
1. Ensure you have [devenv](https://devenv.sh) installed.
2. Clone this repository and step into the project directory:
   ```bash
   git clone https://github.com
   cd anvaya-core
   ```
3. Initialize the development shell and launch the API runtime container directly:
   ```bash
   devenv up
   ```
The engine will instantly set up its isolated `/AppData` flat-file JSON datastore directory and boot up the hot-reloaded Web API listener on `http://localhost:5000`.

---

## 🔌 Core API Documentation Contracts

### 1. Ingest Global Business Event
Clients emit user actions directly to this unified hook endpoint.

* **Endpoint:** `POST /api/v1/events`
* **Sample Payload (Cross-Border Retail / Premium Activity):**

```json
{
  "tenantId": "tenant_global_retail",
  "customerId": "cust_india_traveler",
  "eventName": "PurchaseCompleted",
  "value": 8000.00,
  "currency": "INR",
  "attributes": {
    "Category": "Premium"
  }
}
```

* **Engine Response Return:**
```json
{
  "message": "Event processed universally by Anvaya Core Engine.",
  "pointsAwarded": 200,
  "newBalance": 300,
  "calculatedTier": "Silver",
  "processedLocalTime": "2026-10-03 01:19:42"
}
```

### 2. Retrieve Member Summary Overview
Extract a holistic member object snapshot to bind live UI user interface cards.

* **Endpoint:** `GET /api/v1/members/{customerId}?tenantId={tenantId}`
* **Response Return:**
```json
{
  "customerId": "cust_india_traveler",
  "tenantId": "tenant_global_retail",
  "pointsBalance": 300,
  "currentTier": "Silver",
  "isPremiumSubscriber": true,
  "householdId": "house_sharma_family",
  "preferredLanguage": "en-IN"
}
```

---

## 🏛️ Code Architecture Philosophy

Anvaya strips away domain-specific definitions by relying on an abstract **Rules Engine Middleware Strategy**. This approach processes ingestion requests linearly:

