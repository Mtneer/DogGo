using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DogGo.Repositories
{
    public class WalkerRepository : IWalkerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkerRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walker> GetAllWalkers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], ImageUrl, NeighborhoodId
                        FROM Walker
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }

        public Walker GetWalkerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id AS walkerId, 
	                                           w.[Name] AS walkerName, 
                                               w.ImageUrl,
	                                           w.NeighborhoodId,
                                               Walks.Id AS walksId,
	                                           Walks.Date, 
	                                           Walks.Duration,
                                               d.Id AS dogId,
                                               d.OwnerId AS ownerId,
                                               o.[Name] AS clientName
	                                      FROM Walker w
	                                 LEFT JOIN Walks on Walks.WalkerId = w.Id
                                     LEFT JOIN Dog d on Walks.DogId = d.Id
                                     LEFT JOIN Owner o on d.OwnerId = o.Id
	                                     WHERE w.Id = @id;
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    Walker walker = new Walker();
                    if (reader.Read())
                    {
                        if (walker.Id == 0)
                        { 
                            walker.Id = reader.GetInt32(reader.GetOrdinal("walkerId"));
                            walker.Name = reader.GetString(reader.GetOrdinal("walkerName"));
                            walker.ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
                            walker.NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"));
                            if (reader.IsDBNull(reader.GetOrdinal("walksId")) == false)
                            {
                                walker.Walks = new List<Walks>();
                                Owner owner = new Owner
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ownerId")),
                                    Name = reader.GetString(reader.GetOrdinal("clientName"))
                                };
                                Walks walk = new Walks
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("walksId")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                    Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                    Client = owner
                                };
                                walker.Walks.Add(walk);
                            }
                         
                        } else
                        {
                            Owner owner = new Owner
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ownerId")),
                                Name = reader.GetString(reader.GetOrdinal("clientName"))
                            };
                            Walks walk = new Walks
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("walksId")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                Client = owner
                            };
                            walker.Walks.Add(walk);
                        }

                        reader.Close();
                        return walker;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }

        public List<Walker> GetWalkersInNeighborhood(int neighborhoodId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], ImageUrl, NeighborhoodId
                        FROM Walker
                        WHERE NeighborhoodId = @neighborhoodId
                        ";

                    cmd.Parameters.AddWithValue("@neighborhoodId", neighborhoodId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {
                        Walker walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        walkers.Add(walker);
                    }

                    reader.Close();

                    return walkers;
                }
            }
        }
    }
}
