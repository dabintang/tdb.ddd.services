//分组
function groupBy(list, fn) {
    const groups = {};
    list.forEach(function (o) {
        // const group = JSON.stringify(fn(o));
		const group = fn(o);
        groups[group] = groups[group] || [];
        groups[group].push(o);
    });
    // return Object.keys(groups).map(function (group) {
    //     return groups[group];
    // });
    return groups;
}

export default groupBy;


// let links = [
//             { source: 'test1', target: 'test1', value: 10 },
//             { source: 'test1', target: 'test2', value: 30 },
//             { source: 'test1', target: 'test3', value: 40 },
//             { source: 'test1', target: 'test4', value: 20 }
// ]
// let groupData = groupBy(links, (link) => {
//             return link.source
// })
// console.log(groupData)
// 返回结果
//{
//     "test1":[
//         { source: "test1", target: "test1", value: 10 },
//         { source: "test1", target: "test2", value: 30 },
//         { source: "test1", target: "test3", value: 40 },
//         { source: "test1", target: "test4", value: 20 }
//     ]
// }
