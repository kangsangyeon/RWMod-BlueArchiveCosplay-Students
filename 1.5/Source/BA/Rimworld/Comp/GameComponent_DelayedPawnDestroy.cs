using System.Collections.Generic;
using Verse;

namespace BA
{
    public class GameComponent_DelayedPawnDestroy : GameComponent
    {
        private List<Pawn> _destroyedPawns = new();

        public IReadOnlyList<Pawn> DestroyedPawns => _destroyedPawns;

        public GameComponent_DelayedPawnDestroy(Game game)
        {
            if (!ModsConfig.IsActive(Const.ModPackageId))
                return;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _destroyedPawns, nameof(_destroyedPawns));
        }

        public void TryAdd(Pawn pawn)
        {
            if (!_destroyedPawns.Contains(pawn))
                _destroyedPawns.Add(pawn);
        }

        public void Remove(Pawn pawn)
        {
            _destroyedPawns.Remove(pawn);
        }

        public override void GameComponentTick()
        {
            if (_destroyedPawns.Count == 0)
                return;
            _destroyedPawns.RemoveAll(p => p.Destroyed);
            foreach (var p in _destroyedPawns)
                p.Destroy();
            _destroyedPawns.Clear();
        }
    }
}