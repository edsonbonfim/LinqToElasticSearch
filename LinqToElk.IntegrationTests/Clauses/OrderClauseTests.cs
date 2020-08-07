﻿using System.Linq;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace LinqToElk.IntegrationTests.Clauses
{
    public class OrderClauseTests: IntegrationTestsBase<SampleData>
    {
        [Fact]
        public void OrderAscObjects()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>(11).ToList();

            foreach (var data in datas)
            {
                data.Age = 30;
                data.Can = true;
            }

            datas[7].Age = 23;
            

            Bulk(datas);
            
            ElasticClient.Indices.Refresh();
            
            //When
            var results = Sut.OrderBy(x => x.Age).ToList();

            //Then
            results.Count().Should().Be(11);
            results.First().Age.Should().Be(23);
        }
        
        [Fact]
        public void OrderDescObjects()
        {
            //Given
            var datas = Fixture.CreateMany<SampleData>(11).ToList();

            foreach (var data in datas)
            {
                data.Age = 30;
                data.Can = true;
            }

            datas[7].Age = 23;
            

            Bulk(datas);
            
            ElasticClient.Indices.Refresh();
            
            //When
            var results = Sut.OrderByDescending(x => x.Age).ToList();

            //Then
            results.Count().Should().Be(11);
            results[10].Age.Should().Be(23);
        }
    }
}