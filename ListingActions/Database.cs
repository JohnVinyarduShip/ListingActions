using System;
using System.Collections.Generic;
using System.Linq;
using ListingActions.Entities;
using Newtonsoft.Json;

namespace ListingActions
{
    public interface IDatabase
    {
        void Upsert<TData>(TData data) where TData : BaseEntity;

        IEnumerable<TData> FindAll<TData>(Predicate<TData> predicate)
            where TData : BaseEntity;

        TData Get<TData>(Guid id) where TData : BaseEntity;
        void Delete(Guid id);
        void Clear();
    }

    public class Database : IDatabase
    {
        private readonly IDictionary<Guid, string> _dict = new Dictionary<Guid, string>();

        public void Upsert<TData>(TData data) where TData : BaseEntity
        {
            if (data.Id == Guid.Empty)
            {
                data.Id = Guid.NewGuid();
            }
            _dict[data.Id] = JsonConvert.SerializeObject(data);
        }

        public IEnumerable<TData> FindAll<TData>(Predicate<TData> predicate)
            where TData : BaseEntity
        {
            return _dict
                .SelectMany(x =>
                {
                    try
                    {
                        var data = JsonConvert.DeserializeObject<TData>(x.Value);
                        if (predicate(data)) { return new List<TData> { data }; }
                    }
                    catch { }
                    return Enumerable.Empty<TData>();
                });
        }

        public TData Get<TData>(Guid id) where TData : BaseEntity
        {
            return JsonConvert.DeserializeObject<TData>(_dict[id]);
        }

        public void Delete(Guid id)
        {
            _dict.Remove(id);
        }

        public void Clear()
        {
            _dict.Clear();
        }
    }
}
