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
          rowBound: "false",
        },
        {
          Header: "No. Subjects",
          accessor: "subjectCount",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "No. Consumers",
          accessor: "consumerCount",
          appendChildren: "false",
          rowBound: "false",
        },
        {
          Header: "No. Messages",
          accessor: "messageCount",
          appendChildren: "false",
          rowBound: "false",
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
