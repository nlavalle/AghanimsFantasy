<template>
  <div class="about">
    <div class="row justify-evenly">
      <div class="col col-4-grow">
        <p class="text-white">
          NADCL Fantasy Draft, click the fantasy tab to get started.
          <br />
          Points are calculated using the pattern below
          following the TI fantasy scoring. Certain statistics require parsing the .dem replay files to measure, so they
          won't be involved in the calculation until that is added to the API service that fetches match data.
        </p>
      </div>
    </div>
    <div class="row justify-evenly">
      <q-table table-class="score-table" :columns="headers" :rows="statistics" dense hide-pagination separator="vertical"
        :rows-per-page-options="[0]" />
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
    available: "Requires Replay Parsing"
  },
  {
    name: "Roshan Kill",
    value: "1",
    available: "Requires Replay Parsing"
  },
  {
    name: "Team Fight",
    value: "3",
    available: "Requires Replay Parsing"
  },
  {
    name: "Wards Planted",
    value: "0.5",
    available: "Requires Replay Parsing"
  },
  {
    name: "Camps Stacked",
    value: "0.5",
    available: "Requires Replay Parsing"
  },
  {
    name: "Runes Grabbed",
    value: "0.25",
    available: "Requires Replay Parsing"
  },
  {
    name: "First Blood",
    value: "4.0",
    available: "Requires Replay Parsing"
  },
  {
    name: "Stuns",
    value: "0.05",
    available: "Requires Replay Parsing"
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
.score-table th {
  background-color: #3a404d;
  color: white;
  font-family: Arial, Helvetica, sans-serif;
  font-size: 18px;
}

.score-table tbody td {
  background-color: #192038;
  color: white;
  font-family: Arial, Helvetica, sans-serif;
  font-size: 16px;
}

h3 {
  margin: 40px 0 0;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  display: inline-block;
  margin: 0 10px;
}

a {
  color: #42b983;
}
</style>