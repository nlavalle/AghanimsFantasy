import { ref, computed } from 'vue'

export const collapsed = ref(true)
export const toggleFantasyNavbar = () => (collapsed.value = !collapsed.value)
export const showFantasyNavbar = () => (collapsed.value = false)
export const hideFantasyNavbar = () => (collapsed.value = true)

export const FANTASY_NAVBAR_HEIGHT = 48
export const FANTASY_NAVBAR_HEIGHT_COLLAPSED = 0
export const fantasyNavbarHeight = computed(
  () => `${collapsed.value ? FANTASY_NAVBAR_HEIGHT_COLLAPSED : FANTASY_NAVBAR_HEIGHT}px`
)
