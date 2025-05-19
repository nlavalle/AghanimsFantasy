<template>
    <v-row :class="!display.mobile.value ? 'radar-text-desktop' : `radar-text-mobile`">
        <v-col>
            <v-row justify="center">
                <span>{{ props.title }}</span>
            </v-row>
            <v-row justify="center">
                <Radar
                    :style="{ width: !display.mobile.value ? '200px' : '280px', height: !display.mobile.value ? '180px' : '180px' }"
                    :data="chartData" :options="options">
                </Radar>
            </v-row>
        </v-col>
    </v-row>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { VRow, VCol } from 'vuetify/components';
import { Radar } from 'vue-chartjs'
import {
    Chart as ChartJS,
    RadialLinearScale,
    PointElement,
    LineElement,
    Filler,
    Tooltip,
    Legend
} from 'chart.js'
import { useDisplay } from 'vuetify';

ChartJS.register(
    RadialLinearScale,
    PointElement,
    LineElement,
    Filler,
    Tooltip,
    Legend
)

const props = defineProps({
    title: {
        type: String,
        required: true,
    },
    chartLabels: {
        type: Array,
        required: true,
    },
    chartDataset: {
        type: Array,
        required: true,
    }
})

const display = useDisplay()

const options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
            display: false
        }
    },
    scales: {
        r: {
            backgroundColor: 'rgba(255, 99, 132, 0.2)',
            pointLabels: {
                color: '#fff',
                font: {
                    size: 11
                }
            },
            angleLines: {
                color: 'rgba(255, 99, 132, 0.2)' // Custom color for angle lines
            },
            grid: {
                color: 'rgba(255, 99, 132, 0.2)' // Custom color for grid lines
            },
            ticks: {
                display: false
            }
        }
    }
}



const chartData = ref<any>({
    labels: props.chartLabels,
    datasets: [
        {
            data: props.chartDataset,
            fill: true,
            backgroundColor: 'rgba(255, 99, 132, 0.2)',
            borderColor: 'rgb(255, 99, 132)',
            pointBackgroundColor: 'rgb(255, 99, 132)',
            pointBorderColor: '#fff',
            pointHoverBackgroundColor: '#fff',
            pointHoverBorderColor: 'rgb(255, 99, 132)'
        },
    ]
})

onMounted(() => {
    if (props.chartDataset) {
        chartData.value = {
            labels: props.chartLabels,
            datasets: [
                {
                    data: props.chartDataset,
                    fill: true,
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    borderColor: 'rgb(255, 99, 132)',
                    pointBackgroundColor: 'rgb(255, 99, 132)',
                    pointBorderColor: '#fff',
                    pointHoverBackgroundColor: '#fff',
                    pointHoverBorderColor: 'rgb(255, 99, 132)'
                },
            ]
        }
    }
})

watch(() => props.chartDataset, (updatedDataset) => {
    if (updatedDataset) {
        chartData.value = {
            labels: props.chartLabels,
            datasets: [
                {
                    data: props.chartDataset,
                    fill: true,
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    borderColor: 'rgb(255, 99, 132)',
                    pointBackgroundColor: 'rgb(255, 99, 132)',
                    pointBorderColor: '#fff',
                    pointHoverBackgroundColor: '#fff',
                    pointHoverBorderColor: 'rgb(255, 99, 132)'
                },
            ]
        }
    }
})

</script>

<style scoped>
.radar-text-desktop {
    font-size: 1rem;
}

.radar-text-mobile {
    font-size: 0.8rem;
}
</style>