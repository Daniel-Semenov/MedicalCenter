using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Offices.Domain.Entities;

namespace Offices.Infrastructure.Db;

public static class OfficeConfiguration
{
    public static void Configure()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(Office)))
            return;

        BsonClassMap.RegisterClassMap<Office>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
            
            cm.MapMember(o => o.Id)
                .SetElementName("_id")
                .SetSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            
            cm.MapMember(o => o.City)
                .SetElementName("city")
                .SetIsRequired(true);
            
            cm.MapMember(o => o.Street)
                .SetElementName("street")
                .SetIsRequired(true);
            
            cm.MapMember(o => o.HouseNumber)
                .SetElementName("houseNumber")
                .SetIsRequired(true);
            
            cm.MapMember(o => o.OfficeNumber)
                .SetElementName("officeNumber")
                .SetIsRequired(true);
            
            cm.MapMember(o => o.PhoneNumber)
                .SetElementName("phoneNumber")
                .SetIsRequired(true);
            
            cm.MapMember(o => o.IsActive)
                .SetElementName("isActive")
                .SetIsRequired(true);
            
            cm.MapMember(o => o.PhotoUrl)
                .SetElementName("photoUrl")
                .SetIsRequired(false);
        });
    }
}