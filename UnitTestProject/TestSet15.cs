﻿using Build;

namespace UnitTests
{
    namespace TestSet15
    {
        public interface IPersonRepository
        {
            Person GetPerson(int personId);
        }

        public enum Database : int
        {
            SQL,
            WebService
        }

        public class Person
        {
            readonly IPersonRepository _personRepository;

            public Person(IPersonRepository personRepository)
            {
                _personRepository = personRepository;
            }
        }

        public class SqlDataRepository : IPersonRepository
        {
            public int PersonId { get; }

            public SqlDataRepository(int personId)
            {
                PersonId = personId;
            }

            public Person GetPerson(int personId)
            {
                // get the data from SQL DB and return Person instance.
                return new Person(this);
            }
        }

        public class ServiceDataRepository : IPersonRepository
        {
            public ServiceDataRepository([Injection(typeof(SqlDataRepository), 2018)]IPersonRepository repository)
            {
                Repository = repository;
            }
            public IPersonRepository Repository { get; }
            public Person GetPerson(int personId)
            {
                // get the data from Web service and return Person instance.
                return new Person(this);
            }
        }
    }
}
