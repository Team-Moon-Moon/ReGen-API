using FirebaseAuthDemo.Configurations.Elasticsearch;
using FirebaseAuthDemo.Models;
using FirebaseAuthDemo.Utils.Custom_Exceptions;
using FirebaseAuthDemo.Utils.Elasticsearch.Search_Templates;
using Nest;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Services
{
    public class ElasticsearchService : ISearchService
    {
        private ElasticsearchConfiguration _config;
        private ElasticClient _elasticClient;

        public ElasticsearchService(ElasticsearchConfiguration config, ElasticClient client)
        {
            _config = config;
            _elasticClient = client;
        }

        public async Task<IEnumerable<object>> SearchUnfilteredAsync(string recipeName)
        {
            var searchRequest = BuildUnfilteredSearchRequest(recipeName);

            #region For debugging: shows the raw request body

            byte[] b = new byte[60000];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(b))
            {
                _elasticClient.RequestResponseSerializer.Serialize(searchRequest, ms);
            }
            var rawJson = System.Text.Encoding.Default.GetString(b).Trim('\0');

            #endregion

            var searchResponse = await _elasticClient.SearchAsync<dynamic>(searchRequest);

            if (!searchResponse.IsValid)
            {
                throw searchResponse.OriginalException;
            }

            if (searchResponse.Documents == null || searchResponse.Documents.Count < 1)
            {
                return null;
            }

            return searchResponse.Documents;
        }

        public async Task<IEnumerable<object>> SearchFilteredAsync(string recipeName, IEnumerable<string> includeTags, IEnumerable<string> excludeTags)
        {
            if (recipeName == null)
            {
                throw new ArgumentException("'q' cannot be null.");
            }

            var searchRequest = BuildFilteredSearchRequest(recipeName, includeTags, excludeTags);

            #region **For debugging: shows the raw request body**

            byte[] b = new byte[60000];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(b))
            {
                _elasticClient.RequestResponseSerializer.Serialize(searchRequest, ms);
            }
            var rawJson = System.Text.Encoding.Default.GetString(b).Trim('\0');

            #endregion

            var searchResponse = await _elasticClient.SearchAsync<dynamic>(searchRequest);

            if (!searchResponse.IsValid)
            {
                throw searchResponse.OriginalException;
            }

            if (searchResponse.Documents == null || searchResponse.Documents.Count < 1)
            {
                return null;
            }

            return searchResponse.Documents;
        }

        #region Private helper methods

        private ISearchRequest BuildUnfilteredSearchRequest(string recipeName)
        {
            //var should = CreateShouldClause(recipeName);
            var must = CreateMustClause(recipeName);

            var searchRequest = new SearchRequest
            {
                //Query = should
                Query = must
            };

            return searchRequest;
        }

        private ISearchRequest BuildFilteredSearchRequest(string recipeName, IEnumerable<string> includeTags, IEnumerable<string> excludeTags)
        {
            var must = CreateMustClause(recipeName, includeTags);
            var mustNot = CreateMustNotClause(excludeTags);
            //var should = CreateShouldClause(recipeName);

            var boolQuery =
                new BoolQuery
                {
                    Must = new QueryContainer[] { must },
                    MustNot = new QueryContainer[] { mustNot },
                    //Should = new QueryContainer[] { should }
                };

            var searchRequest = new SearchRequest
            {
                Query = boolQuery
            };

            return searchRequest;
        }

        private QueryContainer CreateMustClause(string recipeName)
        {
            var MustContainer = new QueryContainer();

            var wildIngredient = new WildcardQuery { Field = "ingredients.IngredientName", Value = $"*{recipeName.ToLower()}*" };
            MustContainer |= wildIngredient;

            var fuzzyIngredient = new FuzzyQuery { Field = "ingredients.IngredientName", Value = $"{recipeName.ToLower()}" };
            MustContainer |= fuzzyIngredient;

            var wildRecipe = new WildcardQuery { Field = "name", Value = $"*{recipeName.ToLower()}*" };
            MustContainer |= wildRecipe;

            var fuzzyRecipe = new FuzzyQuery { Field = "name", Value = $"{recipeName.ToLower()}" };
            MustContainer |= fuzzyRecipe;

            var match = new MatchPhraseQuery { Field = "name", Query = recipeName };
            MustContainer |= match;

            return MustContainer;
        }

        private QueryContainer CreateMustClause(string recipeName, IEnumerable<string> tags)
        {
            var MustContainer = CreateMustClause(recipeName);

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    var query = new MatchQuery { Field = "tags", Query = tag };
                    MustContainer &= query;
                }
            }

            return MustContainer;
        }

        private QueryContainer CreateMustNotClause(IEnumerable<string> tags)
        {
            var MustNotContainer = new QueryContainer();

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    var query =
                        new MatchQuery
                        {
                            Field = "tags",
                            Query = tag
                        };

                    MustNotContainer &= query;
                }
            }

            return MustNotContainer;
        }

        //private QueryContainer CreateShouldClause(string recipeName)
        //{
        //    var wild =
        //        new WildcardQuery
        //        {
        //            Field = "name",
        //            Value = $"*{recipeName}*"
        //        };

        //    var fuzzy =
        //        new FuzzyQuery
        //        {
        //            Field = "name",
        //            Value = $"{recipeName}"
        //        };

        //    var ShouldContainer = new QueryContainer();
        //    ShouldContainer |= wild;
        //    ShouldContainer |= fuzzy;

        //    return ShouldContainer;
        //} 

        #endregion
    }




}
