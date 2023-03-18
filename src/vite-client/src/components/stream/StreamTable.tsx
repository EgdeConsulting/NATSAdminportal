import { PaginatedTable, StreamViewButton } from "components";

function StreamTable(props: { streamInfo: any[] }) {
  const columns = [
    {
      Header: "StreamTable",
      columns: [
        {
          Header: "Name",
          accessor: "name",
          appendChildren: "false",
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
        {
          Header: "Data",
          accessor: "data",
          appendChildren: "true",
          rowBound: "true",
        },
      ],
    },
  ];

  return (
    <PaginatedTable columns={columns} data={props.streamInfo}>
      <StreamViewButton content={""} />
    </PaginatedTable>
  );
}
export { StreamTable };
