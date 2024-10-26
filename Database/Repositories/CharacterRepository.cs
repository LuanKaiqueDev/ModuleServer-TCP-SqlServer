using Database.Context;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class CharacterRepository(AppDbContext context)
{
    public async Task<Character?> GetCharacterById(int characterId)
       => await context.Characters.FirstOrDefaultAsync(x => x != null && x.Id == characterId);
}