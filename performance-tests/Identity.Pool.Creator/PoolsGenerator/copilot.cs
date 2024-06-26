///
/// Create a class which will solve the relationships and messages allocation problem.
/// This class should have a constructor containing at least the following:
/// 
/// IPrinter _printer;
/// PoolFileRoot poolFileConfiguration;
///   
/// The first can be used to print the solution.
/// The second can be used to load the pool information for the problem to be solved. See its object Pools.
/// Pools can be of a number of different types. We're interested in pools of types App and Connector.
/// The Pool Type can be determined by calling the pool.IsApp() and Pool.IsConnector() methods.
/// The pool.Amount field indicates the number of Identities to be generated for each pool.
/// The pool.TotalNumberOfMessages indicates the total number of messages an identity should send and receive.
/// The proportion of sent and received messages is determined by the type of the pool each identity belongs to and
/// is defined in the poolFileConfiguration.MessagesSentByConnectorRatio.
/// The pool.NumberOfRelationships is the number of relationships that each identity in that pool should have.
/// A relationship is not directed and connects two identities. The same couple of identities can only have on relationship.
/// Identities of the same type should not establish relationships, i.e, identities of type App should establish relationships
/// with identities of type Connector and vice-versa.
/// A relationship is required to exchange messages, i.e., a given identity i can only send messages to other identities if
/// they have established a relationship with i.
/// 
/// The algorithm should maximize the number of established relationships and exchanged messages.
/// The target for both metrics can be determined by calling the poolFileConfiguration.Pools.ExpectedNumberOfRelationships()
/// and poolFileConfiguration.Pools.ExpectedNumberOfMessages() methods, respectively.
/// 
/// The provided poolFileConfiguration may not be balanced, in the sense that there may be more App identities than 
/// Connector identities and the number of relationships may not match. You must create compensation pools for relationships
/// and messages, while keeping in mind that relationships are required to exchange messages.
/// 
/// The same pair of identities may exchange more than one message.

namespace Backbone.Identity.Pool.Creator.PoolsGenerator;