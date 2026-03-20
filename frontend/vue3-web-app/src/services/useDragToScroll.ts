import { ref, onMounted, onBeforeUnmount } from 'vue'

export function useDragToScroll() {
    const scrollEl = ref<HTMLElement | null>(null)
    let isDragging = false
    let startX = 0
    let startScrollLeft = 0

    const onMouseDown = (e: MouseEvent) => {
        if (!scrollEl.value) return
        isDragging = true
        startX = e.pageX - scrollEl.value.offsetLeft
        startScrollLeft = scrollEl.value.scrollLeft
        document.body.style.userSelect = 'none'
    }

    const onMouseMove = (e: MouseEvent) => {
        if (!isDragging || !scrollEl.value) return
        e.preventDefault()
        const x = e.pageX - scrollEl.value.offsetLeft
        scrollEl.value.scrollLeft = startScrollLeft - (x - startX)
    }

    const onMouseUp = () => {
        isDragging = false
        document.body.style.userSelect = ''
    }

    onMounted(() => {
        scrollEl.value?.addEventListener('mousedown', onMouseDown)
        window.addEventListener('mousemove', onMouseMove)
        window.addEventListener('mouseup', onMouseUp)
    })

    onBeforeUnmount(() => {
        scrollEl.value?.removeEventListener('mousedown', onMouseDown)
        window.removeEventListener('mousemove', onMouseMove)
        window.removeEventListener('mouseup', onMouseUp)
    })

    return { scrollEl }
}
