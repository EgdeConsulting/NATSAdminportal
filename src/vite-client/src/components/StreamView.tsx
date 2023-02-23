import { PaginatedTable, StreamModal } from "components";

function StreamView(props: { streamInfo: any[] }) {
  const columns = [
    {
      Header: "All Streams",
      columns: [
        {
          Header: "Name",
          accessor: "name",
          appendChildren: "true",
        },
        {
          Header: "No. Subjects",
          accessor: "subjectsCount",
          appendChildren: "false",
        },
        {
          Header: "No. Consumers",
          accessor: "consumersCount",
          appendChildren: "false",
        },
        {
          Header: "No. Messages",
          accessor: "messageCount",
          appendChildren: "false",
        },
      ],
    },
  ];

  return (
    <PaginatedTable columns={columns} data={props.streamInfo}>
      <StreamModal content={""} />
    </PaginatedTable>
  );
}
export { StreamView };
