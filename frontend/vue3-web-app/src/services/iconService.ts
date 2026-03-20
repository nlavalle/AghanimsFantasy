export function getPositionIcon(positionInt: number): string {
    return `icons/pos_${positionInt}.png`
}

export function getHeroIcon(heroIconString: string): string | undefined {
    if (!heroIconString) return undefined
    const formatted = heroIconString.replace('npc_dota_hero_', '')
    return `https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/${formatted}.png`
}
