<template>
  <div class="about">
    <div class="row justify-evenly">
      <div class="col col-4-grow">
        <p class="text-white">
          NADCL Fantasy Draft, click the fantasy tab to get started.
          <br />
          Points are calculated using the pattern below
          following the TI fantasy scoring. Certain statistics require parsing the .dem replay files to measure, so the
          unavailable
          metrics won't be involved in the calculation until that is added to the API service that fetches match data
          (hopefully by Season 7).
        </p>
      </div>
    </div>
    <div class="row justify-evenly">
      <q-table class="about-table" table-header-class="about-table-header" table-class="about-table-body" :columns="headers" :rows="statistics" dense
        hide-pagination separator="vertical" :rows-per-page-options="[0]" />
    </div>
  </div>
</template>
  
<script>
export default {
  name: 'AboutPage',
}
</script>

<script setup>
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
    name: "Assist",
    value: "0.15",
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
    name: "Wards Planted",
    value: "0.5",
    available: "No"
  },
  {
    name: "Camps Stacked",
    value: "0.5",
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
  {
    name: "Stuns",
    value: "0.05",
    available: "No"
  }
];
const headers = [
  {
    name: "statistic",
    label: "Statistic",
    field: "name",
    align: "left",
    style: val => val.available == 'Yes' ? 'color: white' : 'color: grey',
  },
  {
    name: "points",
    label: "Points Per",
    field: "value",
    align: "center",
    style: val => val.available == 'Yes' ? val.name == 'Death' ? 'color: red' : 'color: white' : 'color: grey',
  },
  {
    name: "available",
    label: "Availability",
    field: "available",
    align: "center",
    style: val => val.available == 'Yes' ? 'color: white' : 'color: grey',
  }
]
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
  border: 2px solid var(--nadcl-accent-dark);
}

.about-table-header th {
  background-color: var(--nadcl-accent-dark);
  color: var(--nadcl-white);
  font-family: Arial, Helvetica, sans-serif;
  font-size: 16px;
}

.about-table-body tbody tr td {
  color: var(--nadcl-white);
  background-color: var(--nadcl-main-4);
  font-family: Arial, Helvetica, sans-serif;
  font-size: 16px;
}
</style>