import { StreamTable } from "./";

function StreamView(props: { content: any[] }) {
  //Make clickable a prop. Which then in paginatedtable returns a clickable cell?
  //Make new table component
  const columns = [
    {
      Header: "All Streams",
      columns: [
        {
          Header: "Name",
          accessor: "Name",
          hideContent: "false",
          clickable: "true",
        },
        {
          Header: "No. Subjects",
          accessor: "SubjectsCount",
          hideContent: "false",
          clickable: "false",
        },
        {
          Header: "No. Consumers",
          accessor: "ConsumersCount",
          hideContent: "false",
          clickable: "false",
        },
        {
          Header: "No. Messages",
          accessor: "MessageCount",
          hideContent: "false",
          clickable: "false",
        },
      ],
    },
  ];
  return <StreamTable columns={columns} data={props.content} />;
}
export { StreamView };
