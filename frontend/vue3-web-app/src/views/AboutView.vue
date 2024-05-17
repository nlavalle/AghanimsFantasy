<template>
  <div class="about">
    <div class="row justify-evenly">
      <div class="col col-4-grow">
        <p class="text-white">
          Dota 2 Fantasy Draft, click the fantasy tab to get started.
          <br />
          Points are calculated using the pattern below trying to
          follow the TI fantasy scoring. Certain statistics require parsing the .dem replay files to measure, so the
          unavailable
          metrics won't be involved in the calculation until that is added to the API service that fetches match data
          (hopefully by Season 7).
        </p>
      </div>
    </div>
    <div class="row justify-evenly">
      <!-- <q-table class="about-table" table-header-class="about-table-header" table-class="about-table-body" :columns="headers" :rows="statistics" dense
        hide-pagination separator="vertical" :rows-per-page-options="[0]" /> -->
      <DataTable size="small" :value="statistics" tableClass="about-table">
        <Column field="name" header="Name" headerClass="about-table-header" bodyClass="about-table-body"></Column>
        <Column header="Points Per" headerClass="about-table-header" bodyClass="about-table-body"
          bodyStyle="text-align:center">
          <template #body="slotProps">
            <span :style="getPointsPer(slotProps.data)">{{ slotProps.data.value }}</span>
          </template>
        </Column>
        <Column header="Availability" headerClass="about-table-header" bodyClass="about-table-body"
          bodyStyle="text-align:center">
          <template #body="slotProps">
            <span :style="getAvailability(slotProps.data)">{{ slotProps.data.available }}</span>
          </template>
        </Column>
      </DataTable>
    </div>
  </div>
</template>

<script setup lang="ts">
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';

const statistics = [
  {
    name: "Kill",
    value: "0.3",
    available: "Yes"
  },
  {
    name: "Death",
    value: "-0.3",
    available: "Yes"
  },
  {
    name: "Last hit",
    value: "0.003",
    available: "Yes"
  },
  {
    name: "Gold per minute",
    value: "0.002",
    available: "Yes"
  },
  {
    name: "XP per minute",
    value: "0.002",
    available: "Yes"
  },
  {
    name: "Wards Planted",
    value: "0.15",
    available: "Yes"
  },
  {
    name: "Camps Stacked",
    value: "0.5",
    available: "Yes"
  },
  {
    name: "Stuns",
    value: "0.05",
    available: "Yes"
  },
  {
    name: "Tower Kill",
    value: "1",
    available: "No"
  },
  {
    name: "Roshan Kill",
    value: "1",
    available: "No"
  },
  {
    name: "Team Fight",
    value: "3",
    available: "No"
  },
  {
    name: "Runes Grabbed",
    value: "0.25",
    available: "No"
  },
  {
    name: "First Blood",
    value: "4.0",
    available: "No"
  },
];

const getPointsPer = (field) => {
  return field.available == 'Yes' ? field.name == 'Death' ? 'color: red' : 'color: white' : 'color: grey';
}

const getAvailability = (field) => {
  return field.available == 'Yes' ? 'color: white' : 'color: grey';
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style>
.about {
  margin-top: 10px;
  margin-left: 10px;
  margin-right: 10px;
}

.about-table {
  max-width: 375px;
  border: 2px solid var(--surface-border);
  border-radius: 5px;
}

.about-table .p-datatable-thead>tr>th {
  border-right: 1px solid  var(--surface-border);
}

.about-table .p-datatable-tbody>tr>td {
  border-right: 1px solid  var(--surface-border);
}

.about-table .p-datatable-thead>tr>th:last-child,
.about-table .p-datatable-tbody>tr>td:last-child {
  border-right: none;
  /* Remove the border from the last column */
}

.about-table .p-datatable-tbody>tr:not(:last-child)>td {
  border-bottom: none;
  /* Remove the bottom border of each cell to hide row gridlines */
}

.about-table-header {
  background-color: var(--surface-border);
  color: var(--aghanims-fantasy-white);
  font-family: Arial, Helvetica, sans-serif;
  font-size: 16px;
}

.about-table-header:not(:first-child) > div {
  justify-content: center;
}

.about-table-body {
  color: var(--aghanims-fantasy-white);
  background-color: var(--surface-card);
  font-family: Arial, Helvetica, sans-serif;
  font-size: 16px;
}
</style>
