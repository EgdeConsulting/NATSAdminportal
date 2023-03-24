import { PaginatedTable, StreamViewButton, IStream } from "components";

function StreamTable(props: { streamInfo: IStream[] }) {
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
      {/* What type is content? Regarding StreamViewButton.tsx */}
    </PaginatedTable>
  );
}
export { StreamTable };
