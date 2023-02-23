import { PaginatedTable, ContentHider } from "components";

function MessageView(props: { messages: any[] }) {
  const columns = [
    {
      Header: "All Messages",
      columns: [
        {
          Header: "Timestamp",
          accessor: "timestamp",
          appendChildren: "false",
        },
        {
          Header: "Subject",
          accessor: "subject",
          appendChildren: "false",
        },
        {
          Header: "Acknowledgement",
          accessor: "acknowledgement",
          appendChildren: "false",
        },
        {
          Header: "Headers",
          accessor: "headers",
          appendChildren: "true",
        },
        {
          Header: "Payload",
          accessor: "payload",
          appendChildren: "true",
        },
      ],
    },
  ];

  return (
    <PaginatedTable columns={columns} data={props.messages}>
      <ContentHider content={""} />
    </PaginatedTable>
  );
}

export { MessageView };
