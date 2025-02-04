using Backbone.Modules.Tokens.Domain.Entities;
using Xunit.Abstractions;

namespace Backbone.Modules.Tokens.Domain.Tests.Tests.Tokens;

public partial class TokenTryToAccessAccessTests
{
    public class TokenAccessTestData : TheoryData<int, TokenProperties>
    {
        public TokenAccessTestData()
        {
            Add(0, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.Anonymous, false, TokenAccessResult.Ok));
            Add(1, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.Anonymous, true, TokenAccessResult.Ok));
            Add(2, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(3, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(4, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.B, false, TokenAccessResult.AllocationAdded));
            Add(5, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(6, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.Ok));
            Add(7, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.Anonymous, true, TokenAccessResult.Ok));
            Add(8, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(9, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(10, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.B, false, TokenAccessResult.AllocationAdded));
            Add(11, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(12, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, true, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(13, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, true, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(14, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, true, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(15, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, true, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(16, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, true, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(17, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.Empty, true, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(18, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Empty, Identity.Anonymous, false, TokenAccessResult.WrongPassword));
            Add(19, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Empty, Identity.Anonymous, true, TokenAccessResult.WrongPassword));
            Add(20, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(21, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(22, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Empty, Identity.B, false, TokenAccessResult.WrongPassword));
            Add(23, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(24, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.WrongPassword));
            Add(25, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.Anonymous, true, TokenAccessResult.WrongPassword));
            Add(26, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(27, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(28, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.B, false, TokenAccessResult.WrongPassword));
            Add(29, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(30, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.WrongPassword));
            Add(31,
                new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.Anonymous, true,
                    TokenAccessResult.WrongPassword)); // an anonymous user cannot have an allocation; this case will never really happen. But the code returns WrongPassword in such a case
            Add(32, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(33, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(34, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.B, false, TokenAccessResult.WrongPassword));
            Add(35, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(36, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Empty, Identity.Anonymous, false, TokenAccessResult.Locked));
            Add(37, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Empty, Identity.Anonymous, true, TokenAccessResult.Locked));
            Add(38, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(39, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(40, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Empty, Identity.B, false, TokenAccessResult.Locked));
            Add(41, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(42, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.Anonymous, false, TokenAccessResult.Locked));
            Add(43, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.Anonymous, true, TokenAccessResult.Locked));
            Add(44, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(45, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(46, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.B, false, TokenAccessResult.Locked));
            Add(47, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(48, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.Anonymous, false, TokenAccessResult.Locked));
            Add(49, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.Anonymous, true, TokenAccessResult.Locked));
            Add(50, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(51, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(52, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.B, false, TokenAccessResult.Locked));
            Add(53, new TokenProperties(Identity.A, false, Identity.Anonymous, Password.X, true, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(54, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Empty, Identity.Anonymous, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(55, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Empty, Identity.Anonymous, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(56, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(57, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(58, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Empty, Identity.B, false, TokenAccessResult.AllocationAdded));
            Add(59, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(60, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Empty, Identity.C, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(61, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Empty, Identity.C, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(62, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(63, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Y, Identity.Anonymous, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(64, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(65, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(66, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Y, Identity.B, false, TokenAccessResult.AllocationAdded));
            Add(67, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(68, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Y, Identity.C, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(69, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, false, Password.Y, Identity.C, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(70, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Empty, Identity.Anonymous, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(71, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Empty, Identity.Anonymous, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(72, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(73, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(74, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(75, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Empty, Identity.C, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(76, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Empty, Identity.C, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(77, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Y, Identity.Anonymous, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(78, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Y, Identity.Anonymous, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(79, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(80, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(81, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(82, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Y, Identity.C, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(83, new TokenProperties(Identity.A, false, Identity.B, Password.Empty, true, Password.Y, Identity.C, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(84, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Empty, Identity.Anonymous, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(85, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Empty, Identity.Anonymous, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(86, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(87, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(88, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Empty, Identity.B, false, TokenAccessResult.WrongPassword));
            Add(89, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(90, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Empty, Identity.C, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(91, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Empty, Identity.C, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(92, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(93, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.Anonymous, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(94, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(95, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(96, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.B, false, TokenAccessResult.WrongPassword));
            Add(97, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(98, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.C, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(99, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.C, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(100, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(101, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.Anonymous, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(102, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(103, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(104, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.B, false, TokenAccessResult.WrongPassword));
            Add(105, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(106, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.C, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(107, new TokenProperties(Identity.A, false, Identity.B, Password.X, false, Password.Y, Identity.C, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(108, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Empty, Identity.Anonymous, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(109, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Empty, Identity.Anonymous, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(110, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(111, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(112, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Empty, Identity.B, false, TokenAccessResult.Locked));
            Add(113, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(114, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Empty, Identity.C, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(115, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Empty, Identity.C, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(116, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Y, Identity.Anonymous, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(117, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Y, Identity.Anonymous, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(118, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(119, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(120, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Y, Identity.B, false, TokenAccessResult.Locked));
            Add(121, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(122, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Y, Identity.C, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(123, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.Y, Identity.C, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(124, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.X, Identity.Anonymous, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(125, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.X, Identity.Anonymous, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(126, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.X, Identity.A, false, TokenAccessResult.Ok));
            Add(127, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.X, Identity.A, true, TokenAccessResult.Ok));
            Add(128, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.X, Identity.B, false, TokenAccessResult.Locked));
            Add(129, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.X, Identity.B, true, TokenAccessResult.Ok));
            Add(130, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.X, Identity.C, false, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(131, new TokenProperties(Identity.A, false, Identity.B, Password.X, true, Password.X, Identity.C, true, TokenAccessResult.ForIdentityDoesNotMatch));
            Add(132, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(133, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(134, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(135, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(136, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.B, false, TokenAccessResult.Expired));
            Add(137, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(138, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(139, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(140, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(141, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(142, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.B, false, TokenAccessResult.Expired));
            Add(143, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(144, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, true, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(145, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, true, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(146, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, true, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(147, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, true, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(148, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, true, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(149, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.Empty, true, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(150, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Empty, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(151, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Empty, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(152, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(153, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(154, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Empty, Identity.B, false, TokenAccessResult.Expired));
            Add(155, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(156, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(157, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(158, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(159, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(160, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.B, false, TokenAccessResult.Expired));
            Add(161, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(162, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(163,
                new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.Anonymous, true,
                    TokenAccessResult.Expired)); // an anonymous user cannot have an allocation; this case will never really happen. But the code returns WrongPassword in such a case
            Add(164, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(165, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(166, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.B, false, TokenAccessResult.Expired));
            Add(167, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(168, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Empty, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(169, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Empty, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(170, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(171, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(172, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Empty, Identity.B, false, TokenAccessResult.Expired));
            Add(173, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(174, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(175, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(176, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(177, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(178, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.B, false, TokenAccessResult.Expired));
            Add(179, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(180, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(181, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(182, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(183, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(184, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.B, false, TokenAccessResult.Expired));
            Add(185, new TokenProperties(Identity.A, true, Identity.Anonymous, Password.X, true, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(186, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Empty, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(187, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Empty, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(188, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(189, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(190, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Empty, Identity.B, false, TokenAccessResult.Expired));
            Add(191, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(192, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Empty, Identity.C, false, TokenAccessResult.Expired));
            Add(193, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Empty, Identity.C, true, TokenAccessResult.Expired));
            Add(194, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(195, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Y, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(196, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(197, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(198, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Y, Identity.B, false, TokenAccessResult.Expired));
            Add(199, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(200, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Y, Identity.C, false, TokenAccessResult.Expired));
            Add(201, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, false, Password.Y, Identity.C, true, TokenAccessResult.Expired));
            Add(202, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Empty, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(203, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Empty, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(204, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(205, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(206, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(207, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Empty, Identity.C, false, TokenAccessResult.Expired));
            Add(208, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Empty, Identity.C, true, TokenAccessResult.Expired));
            Add(209, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Y, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(210, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Y, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(211, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(212, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(213, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(214, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Y, Identity.C, false, TokenAccessResult.Expired));
            Add(215, new TokenProperties(Identity.A, true, Identity.B, Password.Empty, true, Password.Y, Identity.C, true, TokenAccessResult.Expired));
            Add(216, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Empty, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(217, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Empty, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(218, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(219, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(220, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Empty, Identity.B, false, TokenAccessResult.Expired));
            Add(221, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(222, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Empty, Identity.C, false, TokenAccessResult.Expired));
            Add(223, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Empty, Identity.C, true, TokenAccessResult.Expired));
            Add(224, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(225, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(226, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(227, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(228, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.B, false, TokenAccessResult.Expired));
            Add(229, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(230, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.C, false, TokenAccessResult.Expired));
            Add(231, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.C, true, TokenAccessResult.Expired));
            Add(232, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(233, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(234, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(235, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(236, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.B, false, TokenAccessResult.Expired));
            Add(237, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(238, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.C, false, TokenAccessResult.Expired));
            Add(239, new TokenProperties(Identity.A, true, Identity.B, Password.X, false, Password.Y, Identity.C, true, TokenAccessResult.Expired));
            Add(240, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Empty, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(241, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Empty, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(242, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Empty, Identity.A, false, TokenAccessResult.Ok));
            Add(243, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Empty, Identity.A, true, TokenAccessResult.Ok));
            Add(244, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Empty, Identity.B, false, TokenAccessResult.Expired));
            Add(245, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Empty, Identity.B, true, TokenAccessResult.Ok));
            Add(246, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Empty, Identity.C, false, TokenAccessResult.Expired));
            Add(247, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Empty, Identity.C, true, TokenAccessResult.Expired));
            Add(248, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Y, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(249, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Y, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(250, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Y, Identity.A, false, TokenAccessResult.Ok));
            Add(251, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Y, Identity.A, true, TokenAccessResult.Ok));
            Add(252, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Y, Identity.B, false, TokenAccessResult.Expired));
            Add(253, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Y, Identity.B, true, TokenAccessResult.Ok));
            Add(254, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Y, Identity.C, false, TokenAccessResult.Expired));
            Add(255, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.Y, Identity.C, true, TokenAccessResult.Expired));
            Add(256, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.X, Identity.Anonymous, false, TokenAccessResult.Expired));
            Add(257, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.X, Identity.Anonymous, true, TokenAccessResult.Expired));
            Add(258, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.X, Identity.A, false, TokenAccessResult.Ok));
            Add(259, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.X, Identity.A, true, TokenAccessResult.Ok));
            Add(260, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.X, Identity.B, false, TokenAccessResult.Expired));
            Add(261, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.X, Identity.B, true, TokenAccessResult.Ok));
            Add(262, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.X, Identity.C, false, TokenAccessResult.Expired));
            Add(263, new TokenProperties(Identity.A, true, Identity.B, Password.X, true, Password.X, Identity.C, true, TokenAccessResult.Expired));

            // Add(_, new TokenProperties(Identity.A, Identity.Anonymous, Password.Empty, true, Password.Empty, Identity.Anonymous, false, TokenAccessResult.Locked)); // a Token without a password cannot be locked
            // Add(_, new TokenProperties(Identity.A, Identity.Anonymous, Password.Empty, true, Password.Empty, Identity.Anonymous, true, TokenAccessResult.Locked)); // a Token without a password cannot be locked
            // Add(_, new TokenProperties(Identity.A, Identity.Anonymous, Password.Empty, true, Password.Empty, Identity.B, false, TokenAccessResult.Locked)); // a Token without a password cannot be locked
            // Add(_, new TokenProperties(Identity.A, Identity.Anonymous, Password.Empty, true, Password.Y, Identity.Anonymous, false, TokenAccessResult.Locked)); // a Token without a password cannot be locked
            // Add(_, new TokenProperties(Identity.A, Identity.Anonymous, Password.Empty, true, Password.Y, Identity.Anonymous, true, TokenAccessResult.Locked)); // a Token without a password cannot be locked
            // Add(_, new TokenProperties(Identity.A, Identity.Anonymous, Password.Empty, true, Password.Y, Identity.B, false, TokenAccessResult.Locked)); // a Token without a password cannot be locked
            // Add(_, new TokenProperties(Identity.A, Identity.B, Password.Empty, true, Password.Empty, Identity.B, false, TokenAccessResult.Locked)); // a Token without a password cannot be locked
            // Add(_, new TokenProperties(Identity.A, Identity.B, Password.Empty, true, Password.Y, Identity.B, false, TokenAccessResult.Locked)); // a Token without a password cannot be locked
        }
    }

    public class TokenProperties : IXunitSerializable
    {
        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public TokenProperties()
        {
        }

        public TokenProperties(Identity createdBy, bool isExpired, Identity forIdentity, Password definedPassword, bool isLocked, Password passwordOnGet, Identity activeIdentity, bool hasAllocation,
            TokenAccessResult expectedResult)
        {
            CreatedBy = createdBy;
            IsExpired = isExpired;
            ForIdentity = forIdentity;
            DefinedPassword = definedPassword;
            IsLocked = isLocked;
            PasswordOnGet = passwordOnGet;
            ActiveIdentity = activeIdentity;
            HasAllocation = hasAllocation;
            ExpectedResult = expectedResult;
        }

        public Identity CreatedBy { get; private set; }
        public bool IsExpired { get; private set; }
        public Identity ForIdentity { get; private set; }
        public Password DefinedPassword { get; private set; }
        public bool IsLocked { get; private set; }

        public Password PasswordOnGet { get; private set; }
        public Identity ActiveIdentity { get; private set; }
        public bool HasAllocation { get; private set; }

        public TokenAccessResult ExpectedResult { get; private set; }

        public void Deserialize(IXunitSerializationInfo info)
        {
            CreatedBy = info.GetValue<Identity>(nameof(CreatedBy));
            IsExpired = info.GetValue<bool>(nameof(IsExpired));
            ForIdentity = info.GetValue<Identity>(nameof(ForIdentity));
            DefinedPassword = info.GetValue<Password>(nameof(DefinedPassword));
            IsLocked = info.GetValue<bool>(nameof(IsLocked));
            PasswordOnGet = info.GetValue<Password>(nameof(PasswordOnGet));
            ActiveIdentity = info.GetValue<Identity>(nameof(ActiveIdentity));
            HasAllocation = info.GetValue<bool>(nameof(HasAllocation));
            ExpectedResult = info.GetValue<TokenAccessResult>(nameof(ExpectedResult));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(CreatedBy), CreatedBy);
            info.AddValue(nameof(IsExpired), IsExpired);
            info.AddValue(nameof(ForIdentity), ForIdentity);
            info.AddValue(nameof(DefinedPassword), DefinedPassword);
            info.AddValue(nameof(IsLocked), IsLocked);
            info.AddValue(nameof(PasswordOnGet), PasswordOnGet);
            info.AddValue(nameof(ActiveIdentity), ActiveIdentity);
            info.AddValue(nameof(HasAllocation), HasAllocation);
            info.AddValue(nameof(ExpectedResult), ExpectedResult);
        }
    }

    public enum Identity
    {
        Anonymous,
        A,
        B,
        C
    }

    public enum Password
    {
        Empty,
        X,
        Y
    }
}
