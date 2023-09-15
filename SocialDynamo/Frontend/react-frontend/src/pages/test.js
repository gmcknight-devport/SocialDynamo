// import React from 'react';
// import CommentPopup from '../components/modals/comments-popup'; 

// // Dummy test data for UserProfileHeader
// const testData = [
//     {
//         authorId: 'User1',
//         commentId: 'Comment1',
//         comment: 'a load of comment stuff',
//         postedAt: '2023/10/09',
//         likes: [
//           {
//             id: '29eid029',
//             userId: 'rr1',
//           }
//         ]
//       },
//       {
//         authorId: 'User2',
//         commentId: 'Comment2',
//         comment: 'more comments here',
//         postedAt: '2023/10/10',
//         likes: [
//           {
//             id: '8y6u2kd',
//             userId: 'testid1',
//           },
//           {
//             id: '3dfg90d',
//             userId: 'aa4',
//           },
//           {
//             id: 'testid1',
//             userId: 'bb5',
//           }
//         ]
//       },
//       {
//         authorId: 'User3',
//         commentId: 'Comment3',
//         comment: 'lots of comments',
//         postedAt: '2023/10/11',
//         likes: [
//           {
//             id: '9lsk1jd',
//             userId: 'cc6',
//           }
//         ]
//       },
//       {
//         authorId: 'User4',
//         commentId: 'Comment4',
//         comment: 'some more comments',
//         postedAt: '2023/10/12',
//         likes: [
//           {
//             id: '2lsk3df',
//             userId: 'dd7',
//           },
//           {
//             id: '67as1lj',
//             userId: 'ee8',
//           },
//           {
//             id: '92al3jd',
//             userId: 'ff9',
//           }
//         ]
//       },
//       {
//         authorId: 'User5',
//         commentId: 'Comment5',
//         comment: 'commenting away',
//         postedAt: '2023/10/13',
//         likes: [
//           {
//             id: 'efsefsef',
//             userId: 'testid1',
//           },
//           {
//             id: '31skjd2',
//             userId: 'hh11',
//           },
//           {
//             id: '53kkas1',
//             userId: 'ii12',
//           },
//           {
//             id: '01as91k',
//             userId: 'jj13',
//           }
//         ]
//       }
//     ];
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 1',
// //     //     userid: 'user1',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 2',
// //     //     userid: 'user2',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 3',
// //     //     userid: 'user3',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 4',
// //     //     userid: 'user4',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 4',
// //     //     userid: 'user4',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 5',
// //     //     userid: 'user5',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 6',
// //     //     userid: 'user6',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 7',
// //     //     userid: 'user7',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 8',
// //     //     userid: 'user8',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 9',
// //     //     userid: 'user9',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 10',
// //     //     userid: 'user10',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 11',
// //     //     userid: 'user11',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 1',
// //     //     userid: 'user1',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 2',
// //     //     userid: 'user2',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 3',
// //     //     userid: 'user3',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 4',
// //     //     userid: 'user4',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 4',
// //     //     userid: 'user4',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 5',
// //     //     userid: 'user5',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 6',
// //     //     userid: 'user6',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 7',
// //     //     userid: 'user7',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 8',
// //     //     userid: 'user8',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 9',
// //     //     userid: 'user9',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 10',
// //     //     userid: 'user10',
// //     // },
// //     // {
// //     //     profilePictureBlob: null,
// //     //     name: 'User 11',
// //     //     userid: 'user11',
// //     // }
// //     // Add more test data rows as needed
// // ];

// export default function TestPage() {
//     return (
//         <div>
//             {/* Render the UserPopup component with the test data */}
//             <CommentPopup title="Comments" commentData={testData} />
//         </div>
//     );
// }
