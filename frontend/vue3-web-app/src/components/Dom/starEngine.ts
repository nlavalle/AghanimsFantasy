/**
 * Shared star engine — all registered canvases share the same star positions
 * in viewport-space coordinates, so stars appear to move continuously across
 * separate elements on screen.
 */

interface Star {
  x: number
  y: number
  size: number
  speed: number
  baseOpacity: number
  twinklePhase: number
  twinkleSpeed: number
}

interface CanvasEntry {
  el: HTMLCanvasElement
  left: number
  top: number
  width: number
  height: number
}

const STAR_COUNT = 1026
const stars: Star[] = []
let initialized = false
let animId: number | null = null
const canvases: CanvasEntry[] = []

function initStars() {
  const w = window.innerWidth
  const h = window.innerHeight
  for (let i = 0; i < STAR_COUNT; i++) {
    stars.push({
      x: Math.random() * w,
      y: Math.random() * h,
      size: Math.random() * 1.2 + 0.2,
      speed: Math.random() * 0.18 + 0.05,
      baseOpacity: Math.random() * 0.55 + 0.2,
      twinklePhase: Math.random() * Math.PI * 2,
      twinkleSpeed: Math.random() * 0.0008 + 0.0003,
    })
  }
}

function tick(t: number) {
  const vw = window.innerWidth
  const vh = window.innerHeight

  // Move stars at ~45° (up-right diagonal)
  for (const s of stars) {
    s.x += s.speed * 0.707
    s.y -= s.speed * 0.707
    if (s.y < -4) { s.y = vh + 4; s.x = Math.random() * vw }
    if (s.x > vw + 4) { s.x = -4; s.y = Math.random() * vh }
  }

  // Draw each registered canvas — only stars within its viewport rect
  for (const entry of canvases) {
    const { el, left, top, width, height } = entry
    const ctx = el.getContext('2d')
    if (!ctx) continue
    ctx.clearRect(0, 0, width, height)

    for (const s of stars) {
      // Convert viewport coords to canvas-local coords
      const lx = s.x - left
      const ly = s.y - top
      const margin = s.size * 4
      if (lx < -margin || lx > width + margin || ly < -margin || ly > height + margin) continue

      const twinkle = 0.75 + 0.25 * Math.sin(t * s.twinkleSpeed + s.twinklePhase)
      const op = s.baseOpacity * twinkle

      // Soft glow halo
      const grd = ctx.createRadialGradient(lx, ly, 0, lx, ly, s.size * 3.5)
      grd.addColorStop(0, `rgba(200, 215, 255, ${op})`)
      grd.addColorStop(1, 'rgba(160, 180, 255, 0)')
      ctx.beginPath()
      ctx.arc(lx, ly, s.size * 3.5, 0, Math.PI * 2)
      ctx.fillStyle = grd
      ctx.fill()

      // Bright core
      ctx.beginPath()
      ctx.arc(lx, ly, s.size, 0, Math.PI * 2)
      ctx.fillStyle = `rgba(230, 240, 255, ${Math.min(op * 1.5, 1)})`
      ctx.fill()
    }
  }

  animId = requestAnimationFrame(tick)
}

export function registerCanvas(el: HTMLCanvasElement) {
  if (!initialized) {
    initStars()
    initialized = true
  }

  const r = el.getBoundingClientRect()
  canvases.push({ el, left: r.left, top: r.top, width: r.width, height: r.height })

  if (canvases.length === 1) {
    animId = requestAnimationFrame(tick)
  }
}

export function unregisterCanvas(el: HTMLCanvasElement) {
  const idx = canvases.findIndex(e => e.el === el)
  if (idx !== -1) canvases.splice(idx, 1)

  if (canvases.length === 0 && animId !== null) {
    cancelAnimationFrame(animId)
    animId = null
  }
}
